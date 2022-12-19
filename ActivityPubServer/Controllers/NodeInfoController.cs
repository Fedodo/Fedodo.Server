using ActivityPubServer.Model.ActivityPub;
using ActivityPubServer.Model.ActivityPub.NodeInfo;
using Microsoft.AspNetCore.Mvc;

namespace ActivityPubServer.Controllers;

public class NodeInfoController : ControllerBase
{
    [HttpGet(".well-known/nodeinfo")]
    public ActionResult<Link> GetNodeInfoLink()
    {
        var link = new NodeLink()
        {
            Rel = "http://nodeinfo.diaspora.software/ns/schema/2.0",
            Href = new Uri($"https://{Environment.GetEnvironmentVariable("DOMAINNAME")}/nodeinfo/2.0")
        };
        
        return Ok(link);
    }

    [HttpGet("nodeinfo/2.0")]
    public ActionResult<NodeInfo> GetNodeInfo()
    {
        var nodeInfo = new NodeInfo
        {
            Version = "2.0",
            Software = new Software()
            {
                Name = "Social-Orca",
                Version = "0.1"
            },
            Protocols = new string[]
            {
                "activitypub"
            },
            Services = null,
            Usage = null,
            OpenRegistrations = false,
            Metadata = null
        };

        return Ok(nodeInfo);
    }
}