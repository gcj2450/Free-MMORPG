package com.assambra.masterserver.app.service;


import com.assambra.masterserver.app.constant.GameConstant;
import com.assambra.masterserver.app.model.CharacterInfoListModel;
import com.assambra.masterserver.app.model.CharacterInfoModel;
import com.assambra.masterserver.app.model.CreateCharacterModel;
import com.assambra.masterserver.app.request.CreateCharacterRequest;
import com.assambra.masterserver.common.entity.Character;
import com.assambra.masterserver.common.entity.CharacterLocation;
import com.assambra.masterserver.common.entity.User;
import com.assambra.masterserver.common.repository.CharacterLocationRepo;
import com.assambra.masterserver.common.repository.CharacterRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Setter
@AllArgsConstructor
@EzySingleton("characterService")
public class CharacterService extends EzyLoggable {

    private final MaxIdService maxIdService;
    private final UserService userService;
    private final CharacterRepo characterRepo;
    private final CharacterLocationRepo characterLocationRepo;


    // < --- Database --->

    public Character getCharacter(Long id)
    {
        return characterRepo.findById(id);
    }

    public void createCharacter(EzyUser ezyUser, CreateCharacterRequest request) {
        User user = userService.getUserByUsername(ezyUser.getName());

        Character character = new Character();
        character.setId(maxIdService.incrementAndGet("character"));
        character.setUserId(user.getId());
        character.setUsername(user.getUsername());
        character.setName(request.getName());
        character.setSex(request.getSex());
        character.setRace(request.getRace());
        character.setModel(request.getModel());
        characterRepo.save(character);

        CharacterLocation characterLocation = new CharacterLocation();
        characterLocation.setId(maxIdService.incrementAndGet("characterLocation"));
        characterLocation.setCharacterId(character.getId());
        characterLocation.setRoom(GameConstant.START_ROOM);
        characterLocation.setPosition(GameConstant.START_POSITION);
        characterLocation.setRotation(GameConstant.START_ROTATION);
        characterLocationRepo.save(characterLocation);
    }

    public Boolean characterExist(String name)
    {
        return characterRepo.findByField("name", name) != null;
    }

    public Long getIdByName(String name){
        Character character = characterRepo.findByField("name", name);
        return character.getId();
    }

    public List<Character> getAllCharactersOfUser (EzyUser ezyUser) {
        User user = userService.getUserByUsername(ezyUser.getName());
        return characterRepo.findListByField("userId", user.getId());
    }

    public List<CharacterLocation> getAllCharacterLocationsOfUser(EzyUser ezyUser) {
        User user = userService.getUserByUsername(ezyUser.getName());
        Character character = characterRepo.findByField("userId", user.getId());

        List<CharacterLocation> characterLocations = new ArrayList<>();
        if(character != null)
            return characterLocationRepo.findListByField("characterId", character.getId());
        else
            return characterLocations;
    }

    public CharacterLocation getCharacterLocation(Long characterId) {
        return characterLocationRepo.findByField("characterId", characterId);
    }

    // < --- Models --->

    public CharacterInfoListModel getCharacterInfoListModel(EzyUser ezyUser) {
        List<Character> allCharacters = getAllCharactersOfUser(ezyUser);
        List<CharacterLocation> allCharacterLocations = getAllCharacterLocationsOfUser(ezyUser);

        List<CharacterInfoModel> characterInfoModel = getListCharacterInfoModel(allCharacters, allCharacterLocations);

        return CharacterInfoListModel.builder()
                .characters(characterInfoModel)
                .build();
    }

    public List<CharacterInfoModel> getListCharacterInfoModel(List<Character> characters, List<CharacterLocation> characterLocations) {
        Map<Long, String> roomMap = characterLocations.stream()
                .collect(Collectors.toMap(CharacterLocation::getCharacterId, CharacterLocation::getRoom));

        List<CharacterInfoModel> answer = characters.stream().map(
                character -> {
                    String room = roomMap.get(character.getId());

                    return CharacterInfoModel.builder()
                            .id(character.getId())
                            .name(character.getName())
                            .sex(character.getSex())
                            .race(character.getRace())
                            .model(character.getModel())
                            .room(room)
                            .build();
                }
        ).collect(Collectors.toList());

        return answer;
    }

    public CreateCharacterModel getCreateCharacterModel(Long id, String result){
        return CreateCharacterModel.builder()
                .id(id)
                .result(result)
                .build();
    }
}