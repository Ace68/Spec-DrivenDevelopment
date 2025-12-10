namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for coordinates.
/// </summary>
public sealed record CoordinatesDto(
    decimal Latitude,
    decimal Longitude);

/// <summary>
/// Data Transfer Object for address.
/// </summary>
public sealed record AddressDto(
    string Street,
    string City,
    string Country,
    string PostalCode,
    CoordinatesDto Coordinates);

/// <summary>
/// Simplified address DTO (for child detail response).
/// </summary>
public sealed record AddressDetailDto(
    string Street,
    string City,
    string Country,
    string PostalCode,
    CoordinatesDto Coordinates);
