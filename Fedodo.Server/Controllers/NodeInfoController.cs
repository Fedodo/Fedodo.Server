using Fedodo.Server.Interfaces;
using Fedodo.Server.Model;
using Fedodo.Server.Model.NodeInfo;
using Microsoft.AspNetCore.Mvc;

namespace Fedodo.Server.Controllers;

public class NodeInfoController : ControllerBase
{
    private readonly ILogger<NodeInfoController> _logger;
    private readonly IMongoDbRepository _repository;

    public NodeInfoController(ILogger<NodeInfoController> logger, IMongoDbRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet(".well-known/nodeinfo")]
    public ActionResult<Link> GetNodeInfoLink()
    {
        _logger.LogTrace($"Entered {nameof(GetNodeInfoLink)} in {nameof(NodeInfoController)}");

        var link = new NodeLink
        {
            Rel = "http://nodeinfo.diaspora.software/ns/schema/2.0",
            Href = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/nodeinfo/2.0")
        };

        return Ok(link);
    }

    [HttpGet("nodeinfo/2.0")]
    public ActionResult<NodeInfo> GetNodeInfo()
    {
        _logger.LogTrace($"Entered {nameof(GetNodeInfo)} in {nameof(NodeInfoController)}");

        var nodeInfo = new NodeInfo
        {
            Version = "2.0",
            Software = new Software
            {
                Name = "Fedodo",
                Version = "0.1"
            },
            Protocols = new[]
            {
                "activitypub"
            },
            Services = new Services
            {
                Outbound = new object[0],
                Inbound = new object[0]
            },
            Usage = new Usage
            {
                LocalPosts = 0, // TODO
                Users = new Users
                {
                    ActiveHalfyear = 1, // TODO
                    ActiveMonth = 1, // TODO
                    Total = 1 // TODO
                }
            },
            OpenRegistrations = false,
            Metadata = new Dictionary<string, string>()
        };

        return Ok(nodeInfo);
    }
}