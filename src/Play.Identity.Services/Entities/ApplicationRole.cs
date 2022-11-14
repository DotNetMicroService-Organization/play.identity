using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Play.Identity.Services.Entities;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<Guid>
{

}