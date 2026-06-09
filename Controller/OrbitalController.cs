using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AegisOrbit.API.Data;
using AegisOrbit.API.Domain.Entities;
using AegisOrbit.API.Domain.Interface;
using AegisOrbit.API.Domain.ValueObjects;
using AegisOrbit.API.Domain.DTOs; 

namespace AegisOrbit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class OrbitalController : ControllerBase
{
    private readonly AegisOrbitContext _context;
    private readonly ICollisionService _collisionService;

    public OrbitalController(AegisOrbitContext context, ICollisionService collisionService)
    {
        _context = context;
        _collisionService = collisionService;
    }

    // 1. GET: api/orbital
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var spaceObjects = await _context.SpacialObjects.ToListAsync();
        return Ok(spaceObjects);
    }

    // 2. POST: api/orbital/satellite
    [HttpPost("satellite")]
    public async Task<IActionResult> CreateSatellite(string name, double mass, double lat, double lon, double alt, double velocity, string satelliteOperator, string frequency)
    {
        var position = new OrbitalCoordinates(lat, lon, alt);
        var satellite = new ActiveSatellite(name, mass, position, velocity, satelliteOperator, frequency);

        _context.SpacialObjects.Add(satellite);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = satellite.Id }, satellite);
    }

    // 3. POST: api/orbital/debris
    [HttpPost("debris")]
    public async Task<IActionResult> CreateDebris(string name, double mass, double lat, double lon, double alt, double velocity, string origin, double sizeMeters)
    {
        var position = new OrbitalCoordinates(lat, lon, alt);
        var debris = new DebrisSpace(name, mass, position, velocity, origin, sizeMeters);

        _context.SpacialObjects.Add(debris);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = debris.Id }, debris);
    }

    // 4. GET: api/orbital/check-collision
    [HttpGet("check-collision")]
    public async Task<IActionResult> CheckCollision(Guid idA, Guid idB)
    {
        var objA = await _context.SpacialObjects.FindAsync(idA);
        var objB = await _context.SpacialObjects.FindAsync(idB);

        if (objA == null || objB == null)
            return NotFound("One or both orbital objects were not found in the radar database.");

        var alert = _collisionService.CheckCollisionRisk(objA, objB);
        return Ok(alert);
    }

    // 5. GET: api/orbital/threat-report
    [HttpGet("threat-report")]
    public async Task<IActionResult> GetThreatReport()
    {
        var allObjects = await _context.SpacialObjects.ToListAsync();
        
        if (allObjects.Count < 2)
            return BadRequest("Not enough orbital objects in the database to perform a collision cross-check.");

        var alerts = new List<CollisionAlertDTO>();

        for (int i = 0; i < allObjects.Count; i++)
        {
            for (int j = i + 1; j < allObjects.Count; j++)
            {
                try
                {
                    var alert = _collisionService.CheckCollisionRisk(allObjects[i], allObjects[j]);

                    if (alert.CollisionProbability > 0)
                    {
                        alerts.Add(alert);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        var sortedReport = alerts.OrderByDescending(a => a.CollisionProbability).ToList();

        return Ok(sortedReport);
    }
} 