using api.Features.UserCredential.Context.Remove.Interfaces;
using api.Features.UserCredential.Context.Update.Interfaces;
using api.Shared.DTOs.UserCredential.Register;
using api.Shared.DTOs.UserCredential.Remove;
using api.Shared.DTOs.UserCredential.Update;
using api.Shared.DTOs.UserCredential.Validate;
using api.Shared.Enums.UserCredential;
using api.Shared.Extensions;
using api.Shared.UserCredential.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.UserCredential.Controllers;

[Route("api/credentials")]
[ApiController]
public class UserCredentialController : ControllerBase
{
    private readonly IRemoveCredentialContextBuilderFactory _removeBuilder;
    private readonly IUpdateCredentialContextBuilderFactory _updateBuilder;
    private readonly IUserCredentialService _credentialService;

    public UserCredentialController(IUserCredentialService credentialService, IUpdateCredentialContextBuilderFactory updateBuilder, IRemoveCredentialContextBuilderFactory removeBuilder)
    {
        _credentialService = credentialService;
        _updateBuilder = updateBuilder;
        _removeBuilder = removeBuilder;
    }

    [Authorize]
    [HttpPost("rfid-tag")]
    public async Task<IActionResult> RegisterRfidTagAsync([FromBody] RegisterCredentialRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            await _credentialService.RegisterCredentialAsync(userId, requestDto.Value, CredentialType.RfidTag);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize]
    [HttpPost("rfid-pin")]
    public async Task<IActionResult> RegisterRfidPinAsync([FromBody] RegisterCredentialRequestDto requestDto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            await _credentialService.RegisterCredentialAsync(userId, requestDto.Value, CredentialType.RfidPin);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("rfid-tag/validate/{value}")]
    public async Task<IActionResult> ValidateRfidTagAsync([FromRoute] string value)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            var resultDto = await _credentialService.ValidateCredentialAsync(value, CredentialType.RfidTag);
            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("rfid-pin")]
    public async Task<IActionResult> RemoveRfidPinAsync([FromBody] RemoveRfidPinRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            const CredentialType type = CredentialType.RfidPin;
            var builder = _removeBuilder.GetBuilder(type);
            var context = builder.Build(requestDto, userId);
            await _credentialService.RemoveCredentialAsync(context, type);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("rfid-tag")]
    public async Task<IActionResult> RemoveRfidTagAsync([FromBody] RemoveRfidTagRequestDto requestDto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            const CredentialType type = CredentialType.RfidTag;
            var builder = _removeBuilder.GetBuilder(type);
            var context = builder.Build(requestDto, userId);
            await _credentialService.RemoveCredentialAsync(context, type);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("rfid-pin")]
    public async Task<IActionResult> UpdateRfidPinAsync([FromBody] UpdateRfidPinRequestDto requestDto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            const CredentialType type = CredentialType.RfidPin;
            var builder = _updateBuilder.GetBuilder(type);
            var context = builder.Build(requestDto, userId);
            await _credentialService.UpdateCredentialAsync(context, type);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("rfid-tag")]
    public async Task<IActionResult> UpdateRfidTagAsync([FromBody] UpdateRfidTagRequestDto requestDto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            const CredentialType type = CredentialType.RfidTag;
            var builder = _updateBuilder.GetBuilder(type);
            var context = builder.Build(requestDto, userId);
            await _credentialService.UpdateCredentialAsync(context, type);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}