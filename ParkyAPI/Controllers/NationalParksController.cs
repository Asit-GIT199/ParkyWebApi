﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();

            var objDto = new List<NationalParkDto>();

            foreach (var obj in objDto)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(objList);
        }

        [HttpGet("{nationalParkId:int}")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if(obj== null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody]NationalParkDto nationalParkDto)
        {
            if(nationalParkDto==null)
            {
                return BadRequest();
            }            
            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if(!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("",$"Something wrong with saving the record{nationalParkObj.Name}");
                return StatusCode(500,ModelState);
            }

            return Ok();
        }
    }
}