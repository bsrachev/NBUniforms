namespace NBUniforms.Services.User
{
    using NBUniforms.Data;

    public class UserService : IUserService
    {
        private readonly NBUniformsDbContext data;

        public UserService(NBUniformsDbContext data)
        {
            this.data = data;
        }

        public string FindByFullName(string userId) => this.data.Users.Find(userId).FullName;

        public string FindByEmail(string userId) => this.data.Users.Find(userId).Email; 
    }
}
