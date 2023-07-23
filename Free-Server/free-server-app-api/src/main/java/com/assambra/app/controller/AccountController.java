package com.assambra.app.controller;

import com.assambra.app.constant.Commands;
import com.assambra.app.constant.ServerVariables;
import com.assambra.app.request.CreateAccountRequest;
import com.assambra.app.request.ForgotPasswordRequest;
import com.assambra.app.service.AccountService;
import com.assambra.common.entity.Account;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.security.EzySHA256;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

@AllArgsConstructor
@EzyRequestController
public class AccountController extends EzyLoggable {

    private final AccountService accountService;
    private final EzyResponseFactory responseFactory;

    @EzyDoHandle(Commands.CREATE_ACCOUNT)
    public void createAccount(EzyUser user, CreateAccountRequest request)
    {
        Account account = accountService.getAccountByUsername(request.getUsername());
        String resultmessage = "";

        if(account == null)
        {
            getLogger().info("Account doesn't exist in db, create new one -> E-Mail: {}, Username: {}, Password: {}", request.getEmail(), request.getUsername(), request.getPassword());
            accountService.createAccount(request.getEmail().toLowerCase(), request.getUsername(), encodePassword(request.getPassword()));
            resultmessage = "successfully";
        }
        else
        {
            if(account.getEmail().equals(request.getEmail().toLowerCase()))
            {
                logger.info("E-Mail already registered");
                resultmessage = "email_already_registered";
            }
            else if(account.getUsername().equals(request.getUsername()))
            {
                logger.info("Username already in use");
                resultmessage ="username_already_in_use";
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_ACCOUNT)
                .param("result", resultmessage)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser user, ForgotPasswordRequest request)
    {
        String password;
        String resultmessage;

        logger.info("Reseive forgot password request for user {}, username or email {}", user.getName(), request.getUsernameOrEMail());

        Account account = accountService.getAccountByUsername(request.getUsernameOrEMail());
        if(account == null)
        {
            account = accountService.getAccountByEMail(request.getUsernameOrEMail().toLowerCase());
        }

        if (account == null)
        {
            logger.info("Forgot password request for user: {}, no username or email address found", user.getName());
            resultmessage = "no_account";
            password = "";
        }
        else
        {
            if(!ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                logger.info("Forgot password request for user: {}, found account: {}, sending password back to client", user.getName(), account.getUsername());
                resultmessage ="sending_email";
                password = account.getPassword();
            }
            else
            {
                logger.info("Forgot password request for user: {}, found account: {}, sending email to: {}",user.getName(), account.getUsername(), account.getEmail());
                resultmessage ="sending_email";
                password = "";
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_PASSWORD)
                .param("result", resultmessage)
                .param("password", password)
                .user(user)
                .execute();
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
