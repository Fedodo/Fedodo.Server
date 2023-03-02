using System.Web;
using Fedodo.Server.Interfaces;
using Fedodo.Server.Model.ActivityPub;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fedodo.Server.Controllers.ActivityPub;

[Route("Shares")]
public class SharesController : ControllerBase
{
    private readonly ILogger<SharesController> _logger;
    private readonly IMongoDbRepository _repository;

    public SharesController(ILogger<SharesController> logger, IMongoDbRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet]
    [Route($"{{postIdUrlEncoded}}")]
    public async Task<ActionResult<OrderedCollection<Activity>>> GetShares(string postIdUrlEncoded, int page)
    {
        _logger.LogTrace($"Entered {nameof(GetShares)} in {nameof(SharesController)}");

        var postId = HttpUtility.UrlDecode(postIdUrlEncoded);

        var builder = Builders<Activity>.Sort;
        var sort = builder.Descending(i => i.Published);

        var filterBuilder = new FilterDefinitionBuilder<Activity>();
        var filter = filterBuilder.Where(i => (string)i.Object == postId);
        
        var sharesOutbox = (await _repository.GetSpecificPaged(DatabaseLocations.OutboxAnnounce.Database,
            DatabaseLocations.OutboxAnnounce.Collection, page, 20, sort, filter)).ToList();        
        var sharesInbox = (await _repository.GetSpecificPaged(DatabaseLocations.InboxAnnounce.Database,
            DatabaseLocations.InboxAnnounce.Collection, page, 20, sort, filter)).ToList();
        var shares = new List<Activity>();
        shares.AddRange(sharesOutbox);
        shares.AddRange(sharesInbox);
        shares.OrderByDescending(i => i.Published);
        var count = 0;
        if (shares.Count < 20)
        {
            count = shares.Count;
        }
        shares = shares.GetRange(0, count);

        var orderedCollection = new OrderedCollectionPage<Activity>
        {
            OrderedItems = shares,
            Id = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/shares/{postId}/?page={page}"),
            Next = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/shares/{postId}/?page={page + 1}"), // TODO
            Prev = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/shares/{postId}/?page={page - 1}"), // TODO
            PartOf = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/shares/{postId}")
        };

        return Ok(orderedCollection);
    }
}