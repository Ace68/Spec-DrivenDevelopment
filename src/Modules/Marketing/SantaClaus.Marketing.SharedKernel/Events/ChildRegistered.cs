using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class ChildRegistered(
    ChildId aggregateId,
    string firstName,
    string lastName,
    DateTime dateOfBirth,
    string country,
    string city,
    string postalCode,
    string street,
    double latitude,
    double longitude) : DomainEvent(aggregateId)
{
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public DateTime DateOfBirth { get; private set; } = dateOfBirth;
    public string Country { get; private set; } = country;
    public string City { get; private set; } = city;
    public string PostalCode { get; private set; } = postalCode;
    public string Street { get; private set; } = street;
    public double Latitude { get; private set; } = latitude;
    public double Longitude { get; private set; } = longitude;
}
