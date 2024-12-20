using FocusLearn.Models.Domain;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Repositories.Implementation
{
    public class LearningMaterialService : ILearningMaterialService
    {
        private readonly FocusLearnDbContext _context;

        public LearningMaterialService(FocusLearnDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати усі навчальні матеріали
        /// </summary>
        public async Task<IEnumerable<LearningMaterialDTO>> GetAllMaterialsAsync()
        {
            return await _context.LearningMaterials
                .Select(l => new LearningMaterialDTO
                {
                    Title = l.Title,
                    Description = l.Description,
                    FileLink = l.FileLink,
                    CreatorId = l.CreatorId
                })
                .ToListAsync();
        }

        /// <summary>
        /// Отримати матеріал за ID
        /// </summary>
        public async Task<LearningMaterialDTO?> GetMaterialByIdAsync(int materialId)
        {
            return await _context.LearningMaterials
                .Where(l => l.MaterialId == materialId)
                .Select(l => new LearningMaterialDTO
                {
                    Title = l.Title,
                    Description = l.Description,
                    FileLink = l.FileLink,
                    CreatorId = l.CreatorId
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Додати навчальний матеріал
        /// </summary>
        public async Task AddMaterialAsync(int creatorId, LearningMaterialDTO materialDto)
        {
            var newMaterial = new LearningMaterial
            {
                CreatorId = creatorId,
                Title = materialDto.Title,
                Description = materialDto.Description,
                FileLink = materialDto.FileLink
            };

            _context.LearningMaterials.Add(newMaterial);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Редагувати навчальний матеріал
        /// </summary>
        public async Task<bool> UpdateMaterialAsync(int id, LearningMaterialDTO materialDto)
        {
            var material = await _context.LearningMaterials.FindAsync(id);

            if (material == null)
                return false;

            material.Title = materialDto.Title;
            material.Description = materialDto.Description;
            material.FileLink = materialDto.FileLink;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Видалити навчальний матеріал
        /// </summary>
        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = await _context.LearningMaterials.FindAsync(id);

            if (material == null)
                return false;

            _context.LearningMaterials.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
