using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Interfaces;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData
    {
        private readonly ILogger<PersonData> _logger;
        private readonly ApplicationDbContext _context;

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>().ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener Persona con ID{id}");
                throw;
            }
        }

        public async Task<Person> CreateAsync(Person Person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(Person);
                await _context.SaveChangesAsync();
                return Person;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la Persona {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Person Person)
        {
            try
            {
                _context.Set<Person>().Update(Person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la Persona {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var Person = await _context.Set<Person>().FindAsync(id);
                if (Person == null)
                    return false;

                _context.Set<Person>().Remove(Person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la Persona {ex.Message}");
                return false;
            }
        }
    }
}

