using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using XSSApplication.Models;
namespace XSSApplication.Controllers
{
    public class PostController : ApiController
    {
        /*
         * Quick struct for postall response, XSS app doesnt really need to be modular
         * */
        public class PostAllResponse {
            public List<string> postTitles;
        }
        private static Dictionary<string, Post> _posts = new Dictionary<string, Post>();
        private static Dictionary<string, Post> _postsGood = new Dictionary<string, Post>(); // non xss posts
        /*
         * Create Post with no XSS possible
         * */
        [Route("api/Post/Create")]
        [HttpPost]
        public HttpResponseMessage CreatePostNonXSS(Post payload) {
            payload.Content = HttpUtility.HtmlEncode(payload.Content);
            payload.Title = HttpUtility.HtmlEncode(payload.Title);
            _postsGood.Add(HttpUtility.HtmlEncode(payload.Title), payload);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Created A New Post") };
        }
        /*
         * Get all good posts i.e non xss
         * */
        [Route("api/Post/all/good")]
        [HttpGet]
        public PostAllResponse GetPostsGood() {
            return new PostAllResponse() { postTitles = _postsGood.Keys.ToList() };
        }
        /*
         * Get a specific good post i.e non xss
         * */
        [Route("api/Post/good/{postTitle}")]
        [HttpGet]
        public Post GetPostGood(string postTitle) {
            return _postsGood[postTitle];
        }
        /*
         * Create xss post
         * */
        [Route("api/Post/Create/XSS")]
        [HttpPost]
        public HttpResponseMessage CreatePost(Post payload) {
            _posts.Add(payload.Title, payload);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Created A New Post") };
        }
        /*
         * Get all bad xss posts
         * */
        [Route("api/Post/all")]
        [HttpGet]
        public PostAllResponse GetPosts() {
            return new PostAllResponse() { postTitles = _posts.Keys.ToList() };
        }
        /*
         * Get a specific bad xss post
         * */
        [Route("api/Post/{postTitle}")]
        [HttpGet]
        public Post GetPost(string postTitle) {
            return _posts[postTitle];
        }
    }
}
