using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReadCom.Models;

namespace ReadCom.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReaderController : ControllerBase
    {
        private List<Reader> readers;

        private readonly ILogger<ReaderController> _logger;

        public ReaderController(ILogger<ReaderController> logger)
        {
            _logger = logger;
            readers = new List<Reader>();
            var reader = new Reader();
            reader.Id = 1;
            reader.Ip = 2;
            reader.ConnectionStatus= 3;
            reader.ReadingStatus = 0;
            readers.Add(reader);
        }

        [HttpGet]
        public IEnumerable<Reader> Get()
        {
            return readers.ToArray();
        }
    }
}