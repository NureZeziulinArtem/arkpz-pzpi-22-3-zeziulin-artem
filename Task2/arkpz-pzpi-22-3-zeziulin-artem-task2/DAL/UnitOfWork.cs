using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ApplicationContext _appContext;
        private bool disposed = false;

        private Repository<Device> _deviceRepository;
        private Repository<Plant> _plantRepository;
        private Repository<Fertilizer> _fertilizerRepository;
        private Repository<CartItem> _cartItemRepository;
        private Repository<DeviceLog> _deviceLogRepository;
        private Repository<WateringReminder> _wateringReminderRepository;

        public Repository<WateringReminder> WateringReminderRepository
        {
            get
            {
                _wateringReminderRepository ??= new Repository<WateringReminder>(_appContext,
                        new Logger<Repository<WateringReminder>>(_loggerFactory));

                return _wateringReminderRepository;
            }
        }

        public Repository<DeviceLog> DeviceLogRepository
        {
            get
            {
                _deviceLogRepository ??= new Repository<DeviceLog>(_appContext,
                        new Logger<Repository<DeviceLog>>(_loggerFactory));

                return _deviceLogRepository;
            }
        }

        public Repository<CartItem> CartItemRepository
        {
            get
            {
                _cartItemRepository ??= new Repository<CartItem>(_appContext,
                        new Logger<Repository<CartItem>>(_loggerFactory));

                return _cartItemRepository;
            }
        }

        public Repository<Fertilizer> FertilizerRepository
        {
            get
            {
                _fertilizerRepository ??= new Repository<Fertilizer>(_appContext,
                        new Logger<Repository<Fertilizer>>(_loggerFactory));

                return _fertilizerRepository;
            }
        }

        public Repository<Plant> PlantRepository
        {
            get
            {
                _plantRepository ??= new Repository<Plant>(_appContext,
                        new Logger<Repository<Plant>>(_loggerFactory));

                return _plantRepository;
            }
        }

        public Repository<Device> DeviceRepository
        {
            get
            {
                _deviceRepository ??= new Repository<Device>(_appContext,
                        new Logger<Repository<Device>>(_loggerFactory));

                return _deviceRepository;
            }
        }

        public UnitOfWork(ApplicationContext context, ILoggerFactory loggerFactory, ILogger<UnitOfWork> logger)
        {
            _appContext = context;
            _loggerFactory = loggerFactory;
            _logger = logger;
        }

        public async Task<int> Save()
        {
            try
            {
                _logger.LogInformation("Saving changes to the database");

                return await _appContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save changes to the database! Error: {errorMessage}", ex.Message);

                throw new Exception($"Fail to save changes to the database: {ex.Message}");
            }
        }

        protected virtual async Task Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    await _appContext.DisposeAsync();
                }
            }

            this.disposed = true;
        }

        public async void Dispose()
        {
            await Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
