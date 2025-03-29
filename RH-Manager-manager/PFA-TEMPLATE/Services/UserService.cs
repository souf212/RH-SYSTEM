using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Mappers;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.viewModels;
using PFA_TEMPLATE.ViewModels;
using PFA_TEMPLATE.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace PFA_TEMPLATE.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public class UserCreationException : Exception
        {
            public UserCreationException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        public class InvalidRoleException : Exception
        {
            public InvalidRoleException(string message) : base(message) { }
        }
        public async Task<List<UserVM>> GetAvailableEmployeesAsync()
        {
            // Get all employees who are not currently assigned to a manager
            var availableEmployees = await _context.Utilisateurs
                .Where(u => u.Role == "Employes" &&
                       !_context.Employes.Any(e => e.ManagerId.HasValue && e.IdUtilisateur == u.Id))
                .Select(u => UserMapper.MapToUserVM(u))
                .ToListAsync();

            return availableEmployees;
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
        public async Task<UserVM> CreateManager(int userId, List<int> teamMemberIds)
        {
            // Create Manager record
            var manager = new Manager
            {
                IdManager = userId,
                IdUtilisateur = userId
            };
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            // Assign team members
            if (teamMemberIds != null && teamMemberIds.Any())
            {
                foreach (var employeeId in teamMemberIds)
                {
                    var employee = await _context.Employes
                        .FirstOrDefaultAsync(e => e.IdEmploye == employeeId);

                    if (employee != null)
                    {
                        employee.ManagerId = userId;
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Return the created manager's UserVM
            var user = await _context.Utilisateurs.FindAsync(userId);
            return UserMapper.MapToUserVM(user);
        }
        public async Task<UserVM> CreateUserAsync(UserVM userVM, List<int> teamMemberIds = null)
        {
            if (string.IsNullOrWhiteSpace(userVM.Email))
                throw new ArgumentException("L'email est obligatoire");

            if (string.IsNullOrWhiteSpace(userVM.Login))
                throw new ArgumentException("Le login est obligatoire");

            if (string.IsNullOrWhiteSpace(userVM.CIN))
                throw new ArgumentException("Le CIN est obligatoire");

            // Check for existing email (case insensitive)
            var emailExists = await _context.Utilisateurs
                .AnyAsync(u => u.Email.ToLower() == userVM.Email.ToLower());
            if (emailExists)
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà");

            // Check for existing login (case insensitive)
            var loginExists = await _context.Utilisateurs
                .AnyAsync(u => u.Login.ToLower() == userVM.Login.ToLower());
            if (loginExists)
                throw new InvalidOperationException("Un utilisateur avec ce login existe déjà");

            // Check for existing CIN (case insensitive)
            var cinExists = await _context.Utilisateurs
                .AnyAsync(u => u.CIN.ToLower() == userVM.CIN.ToLower());
            if (cinExists)
                throw new InvalidOperationException("Un utilisateur avec ce CIN existe déjà");

            ValidateUser(userVM);

            // Get the execution strategy
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Create user
                    var utilisateur = UserMapper.MapToUtilisateur(userVM);
                    utilisateur.Password = _passwordHasher.HashPassword(userVM.Password);

                    _context.Utilisateurs.Add(utilisateur);
                    await _context.SaveChangesAsync();

                    // Create role-specific entities
                    if (userVM.Role == "Manager")
                    {
                        var createdManager = await CreateManager(utilisateur.Id, teamMemberIds);
                      }
                    else
                    {
                        await CreateRoleSpecificEntities(userVM.Role, utilisateur.Id);
                    }
                    await transaction.CommitAsync();

                    userVM.Id = utilisateur.Id;
                    return userVM;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the error (consider using ILogger)
                    throw new UserCreationException("Failed to create user", ex);
                }
            });
        }
        private async Task CreateRoleSpecificEntities(string role, int userId)
        {
            switch (role)
            {
                case "Admin":
                    await CreateAdmin(userId);
                    break;
                case "Employes":
                    await CreateEmployee(userId);
                    break;
                default:
                    throw new InvalidRoleException($"Invalid role specified: {role}");
            }
        }

        private async Task CreateAdmin(int userId)
        {
            var admin = new Administrateur
            {
                IdAdmin = userId,
                IdUtilisateur = userId
            };
            _context.Administrateurs.Add(admin);
            await _context.SaveChangesAsync();
        }
       
        private async Task CreateEmployee(int userId)
        {
            var employe = new Employes
            {
                IdEmploye = userId,
                IdUtilisateur = userId
            };
            _context.Employes.Add(employe);
            await _context.SaveChangesAsync();

            await InitializeLeaveBalance(employe.IdEmploye);
        }

        private async Task InitializeLeaveBalance(int employeeId)
        {
            var congeBalance = new CongeBalance
            {
                IdEmploye = employeeId,
                Annee = DateTime.Now.Year,
                JoursCongesPayesTotal = 30,
                JoursCongesPayesUtilises = 0,
                JoursMaladieTotal = 10,
                JoursMaladieUtilises = 0
            };

            _context.CongeBalances.Add(congeBalance);
            await _context.SaveChangesAsync();
        }

        private void ValidateUser(UserVM userVM)
        {
            if (userVM == null)
                throw new ArgumentNullException(nameof(userVM));

            if (string.IsNullOrWhiteSpace(userVM.Role))
                throw new ArgumentException("Role is required");

            // Add other validations as needed
        }
        public async Task<bool> UpdateUserAsync(UserVM userVM)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                var existingUser = await _context.Utilisateurs.FindAsync(userVM.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Use the execution strategy's built-in transaction handling
                await using var transaction = await _context.Database.BeginTransactionAsync();

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
                    existingUser.Email = userVM.Email;
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
                        else if (oldRole == "Employes")
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
                                IdAdmin = existingUser.Id,
                                IdUtilisateur = existingUser.Id
                            };
                            _context.Administrateurs.Add(admin);
                        }
                        // If new role is User, add to Employes table
                        else if (newRole == "Employes")
                        {
                            var employe = new Employes
                            {
                                IdEmploye = existingUser.Id,
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
            });
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Utilisateurs.FindAsync(id);
            if (user == null) return false;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await RemoveRoleSpecificEntities(user.Role, id);
                _context.Utilisateurs.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                throw;
            }
        }

        private async Task RemoveRoleSpecificEntities(string role, int userId)
        {
            if (role == "Admin")
            {
                var admin = await _context.Administrateurs
                    .FirstOrDefaultAsync(a => a.IdUtilisateur == userId);
                if (admin != null) _context.Administrateurs.Remove(admin);
            }
            else if (role == "Employes")
            {
                var employe = await _context.Employes
                    .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);
                if (employe != null) _context.Employes.Remove(employe);
            }
        }
    }
}