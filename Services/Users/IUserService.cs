﻿namespace NBUniforms.Services.User
{
    public interface IUserService
    {
        string FindByFullName(string userId);

        string FindByEmail(string userId);
    }
}
