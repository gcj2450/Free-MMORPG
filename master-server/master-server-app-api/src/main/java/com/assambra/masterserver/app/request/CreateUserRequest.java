package com.assambra.masterserver.app.request;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import lombok.Data;

@Data
@EzyObjectBinding
public class CreateUserRequest
{
    private String email;
    private String username;
    private String password;
}
