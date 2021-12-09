using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAuthors.Servicios
{
    public interface IServicio
    {
        Guid obetnerScoped();
        Guid obetnerSingleton();
        Guid obetnerTransient();
        void RealizarTarea();
    }

    public class servicioA : IServicio
    {
        private readonly ILogger<servicioA> logger;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;

        public servicioA(ILogger<servicioA> logger, ServicioTransient servicioTransient, ServicioScoped servicioScoped, ServicioSingleton servicioSingleton)
        {
            this.logger = logger;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
        }

        public Guid obetnerTransient() { return servicioTransient.Guid; }
        public Guid obetnerScoped() { return servicioScoped.Guid; }
        public Guid obetnerSingleton() { return servicioSingleton.Guid; }

        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }

    public class servicioB : IServicio
    {
        public Guid obetnerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid obetnerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid obetnerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }

    //ServicioA y servicioB son dos clases distintas pero que implementa la misma interfaz 

    public class ServicioTransient
    {
        public Guid Guid = Guid.NewGuid(); //String Aleatorio
    }

    public class ServicioScoped
    {
        public Guid Guid = Guid.NewGuid(); //String Aleatorio
    }

    public class ServicioSingleton
    {
        public Guid Guid = Guid.NewGuid(); //String Aleatorio
    }
}
