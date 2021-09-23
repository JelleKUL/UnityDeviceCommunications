using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JelleKUL.StreamSocketServer;
using System.Text;

[UnityHttpServer]
public class ExampleRoute : MonoBehaviour {

    [UnityHttpRoute("/example")]
    public void RouteIndex(HttpRequest request, HttpResponse response) {
        response.BodyData = Encoding.UTF8.GetBytes("<html><body>Hello From Unity!</body></html>");
    }
}
