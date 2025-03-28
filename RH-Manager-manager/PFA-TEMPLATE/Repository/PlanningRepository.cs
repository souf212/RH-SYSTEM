﻿using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.viewModels;

namespace PFA_TEMPLATE.Repositories
{
    public class PlanningRepository : IPlanningRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanningRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Planning> GetAllWithDetails()
        {
            return _context.Plannings.Include(p => p.Employe).ToList();
        }


        public async Task<Planning?> GetByIdAsync(int id)
        {
            return await _context.Plannings.Include(p => p.Employe).FirstOrDefaultAsync(p => p.IdPlanning == id);
        }

        public async Task AddAsync(Planning planning)
        {
            await _context.Plannings.AddAsync(planning);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Planning planning)
        {
            _context.Plannings.Update(planning);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Planning planning)
        {
            _context.Plannings.Remove(planning);
            await _context.SaveChangesAsync();
        }
    }
}
