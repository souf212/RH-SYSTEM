using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Mappers;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.viewModels;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserVM> GetUserByIdAsync(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            return utilisateur != null ? UserMapper.MapToUserVM(utilisateur) : null;
        }

        public async Task<IEnumerable<UserVM>> GetAllUsersAsync()
        {
            var utilisateurs = await _context.Utilisateurs.ToListAsync();
            return utilisateurs.Select(UserMapper.MapToUserVM);
        }

        public async Task<UserVM> CreateUserAsync(UserVM userVM)
        {
            // Hash the password
            userVM.Password = HasherProgram.HashPassword(userVM.Password);

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create user
                var utilisateur = UserMapper.MapToUtilisateur(userVM);
                _context.Utilisateurs.Add(utilisateur);
                await _context.SaveChangesAsync();

                // Get the new user ID
                int userId = utilisateur.Id;

                // Based on role, add to the appropriate table
                if (userVM.Role == "Admin")
                {
                    var admin = new Administrateur
                    {
                        IdUtilisateur = userId
                    };
                    _context.Administrateurs.Add(admin);
                }
                else if (userVM.Role == "User")
                {
                    var employe = new Employes
                    {
                        IdUtilisateur = userId
                    };
                    _context.Employes.Add(employe);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Update the ID from the database
                userVM.Id = userId;
                return userVM;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(UserVM userVM)
        {
            var existingUser = await _context.Utilisateurs.FindAsync(userVM.Id);
            if (existingUser == null)
            {
                return false;
            }

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if role has changed
                string oldRole = existingUser.Role;
                string newRole = userVM.Role;

                // Check if password has changed before hashing
                if (!string.IsNullOrEmpty(userVM.Password))
                {
                    existingUser.Password = HasherProgram.HashPassword(userVM.Password);
                }

                // Update user properties
                existingUser.Nom = userVM.Nom;
                existingUser.Prenom = userVM.Prenom;
                existingUser.Adresse = userVM.Adresse;
                existingUser.CIN = userVM.CIN;
                existingUser.Login = userVM.Login;
                existingUser.Telephone = userVM.Telephone;
                existingUser.Role = userVM.Role;

                await _context.SaveChangesAsync();

                // If role changed, update role-specific tables
                if (oldRole != newRole)
                {
                    // If old role was Admin, remove from Admin table
                    if (oldRole == "Admin")
                    {
                        var admin = await _context.Administrateurs
                            .FirstOrDefaultAsync(a => a.IdUtilisateur == existingUser.Id);
                        if (admin != null)
                        {
                            _context.Administrateurs.Remove(admin);
                        }
                    }
                    // If old role was User, remove from Employes table
                    else if (oldRole == "User")
                    {
                        var employe = await _context.Employes
                            .FirstOrDefaultAsync(e => e.IdUtilisateur == existingUser.Id);
                        if (employe != null)
                        {
                            _context.Employes.Remove(employe);
                        }
                    }

                    // If new role is Admin, add to Admin table
                    if (newRole == "Admin")
                    {
                        var admin = new Administrateur
                        {
                            IdUtilisateur = existingUser.Id
                        };
                        _context.Administrateurs.Add(admin);
                    }
                    // If new role is User, add to Employes table
                    else if (newRole == "User")
                    {
                        var employe = new Employes
                        {
                            IdUtilisateur = existingUser.Id
                        };
                        _context.Employes.Add(employe);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return false;
            }

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Delete from role-specific tables first
                if (utilisateur.Role == "Admin")
                {
                    var admin = await _context.Administrateurs
                        .FirstOrDefaultAsync(a => a.IdUtilisateur == id);
                    if (admin != null)
                    {
                        _context.Administrateurs.Remove(admin);
                    }
                }
                else if (utilisateur.Role == "User")
                {
                    var employe = await _context.Employes
                        .FirstOrDefaultAsync(e => e.IdUtilisateur == id);
                    if (employe != null)
                    {
                        _context.Employes.Remove(employe);
                    }
                }

                // Then delete the user
                _context.Utilisateurs.Remove(utilisateur);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}