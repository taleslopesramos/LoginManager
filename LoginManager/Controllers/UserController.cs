using LoginManager.DTO;
using LoginManager.Model;
using LoginManager.Model.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoginManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly LoginManagerDbContext _context;
        public UserController(LoginManagerDbContext context)
        {
            _context = context;
        }
      
        [HttpPost("register")]
        public async Task<ContentResult> PostCreateAsync([FromBody] CreateUserDTO userModel)
        {
            ContentResult response = new ContentResult();
            response.ContentType = "application/json";

            var repeatedEmail = _context.Users
                .Where(u => u.Email == userModel.Email)
                .FirstOrDefault();

            // Valida se já existe o email cadastrado
            if(repeatedEmail != null){
                response.StatusCode = 409;
                response.Content = JsonConvert.SerializeObject(new { mensagem = "E-mail já existente" });
                return response;
            }

            // Adiciona o Usuário
            var guidUser = Guid.NewGuid();
            User user = new User()
            {
                ExternalId = guidUser,
                Email = userModel.Email,
                Name = userModel.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(userModel.Password),
                Created = DateTime.Now,
                LastAccess = DateTime.Now,
                Modified = DateTime.Now
            };
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Adiciona os Profiles
            List<Profile> profiles = userModel.Profiles.Select(p => new Profile()
            {
                ExternalId = Guid.NewGuid(),
                DisplayName = p.DisplayName,
                IdUser = user.Id
            }).ToList();

            await _context.Profiles.AddRangeAsync(profiles);
            await _context.SaveChangesAsync();

            // Gera a Resposta
            var contentResult = new CreatedUserResultDTO() {
                id = user.ExternalId,
                created = user.Created,
                last_login = user.LastAccess,
                modified = user.Modified,
                profiles = profiles.Select(p => new ProfileDTO()
                {
                    displayName = p.DisplayName,
                    id = p.ExternalId
                }).ToList()
            };

            response.Content = JsonConvert.SerializeObject(contentResult);
            response.StatusCode = 200;

            return response;
        }

        [HttpPost("login")]
        public async Task<ContentResult> PostLoginAsync([FromBody] LoginUserDTO loginUser)
        {
            var response = new ContentResult();
            response.ContentType = "application/json";
            
            var user = _context.Users
                .Include(u => u.Profiles)
                .FirstOrDefault(u => u.Email == loginUser.Email);


            // Verifica se o Email está cadastrado
            if(user == null)
            {
                response.StatusCode = 401;
                response.Content = JsonConvert.SerializeObject(new { mensagem = "Usuário e/ou senha inválidos" });
                return response;
            }

            // Verifica se a senha está certa
            if(!BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password))
            {
                response.StatusCode = 401;
                response.Content = JsonConvert.SerializeObject(new { mensagem = "Usuário e/ou senha inválidos" });
                return response;
            }

            // Cria o objeto da response
            var contentResult = new LoggedUserResultDTO()
            {
                id = user.ExternalId,
                created = user.Created,
                last_login = user.LastAccess,
                modified = user.Modified,
                profiles = user.Profiles.Select(p => new ProfileDTO()
                {
                    displayName = p.DisplayName,
                    id = p.ExternalId
                }).ToList()
            };

            // Atualiza o último momento de acesso
            user.LastAccess = DateTime.Now;
            _context.Attach(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.StatusCode = 200;
            response.Content = JsonConvert.SerializeObject(contentResult);

            return response;
        }
    }
}
