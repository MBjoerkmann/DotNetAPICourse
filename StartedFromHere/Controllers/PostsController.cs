using System.Data;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _helper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _helper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [postId],
                [UserId],
                [PostTitle],
                [postContent],
                [PostCreated],
                [PostUpdated] 
                FROM TutorialAppSchema.Posts";

            return _dapper.LoadData<Post>(sql);

        }

        [HttpGet("PostSingle/{PostId}")]
        public Post GetSinglePost(int postId)
        {
            string sql = @"SELECT [postId],
                [UserId],
                [PostTitle],
                [postContent],
                [PostCreated],
                [PostUpdated] 
                FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();

            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("UserPosts/{UserId}")]
        public IEnumerable<Post> GetUserPosts(int userId)
        {
            string sql = @"SELECT [postId],
                [UserId],
                [PostTitle],
                [postContent],
                [PostCreated],
                [PostUpdated] 
                FROM TutorialAppSchema.Posts WHERE UserId = " + userId.ToString();

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(int postId)
        {
            string sql = @"SELECT [postId],
                [UserId],
                [PostTitle],
                [postContent],
                [PostCreated],
                [PostUpdated] 
                FROM TutorialAppSchema.Posts WHERE PostId = " + User.FindFirst("userId")?.Value;

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            // string userId = this.User.FindFirst("userId")?.Value;

            string sql = @"INSERT INTO TutorialAppSchema.Posts (
                UserId, PostTitle, PostContent, PostCreated, PostUpdated) 
                VALUES (@userId , @postTitle , @postContent , @postCreated , @postUpdated)";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            _helper.InsertParametersIntoList(sqlParameters, "@userId", SqlDbType.Int, this.User.FindFirst("userId")?.Value);
            _helper.InsertParametersIntoList(sqlParameters, "@postTitle", SqlDbType.NVarChar, postToAdd.PostTitle);
            _helper.InsertParametersIntoList(sqlParameters, "@postContent", SqlDbType.NVarChar, postToAdd.PostContent);
            _helper.InsertParametersIntoList(sqlParameters, "@postCreated", SqlDbType.DateTime, DateTime.Now);
            _helper.InsertParametersIntoList(sqlParameters, "@postUpdated", SqlDbType.DateTime, DateTime.Now);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Did not add Post corrrectly");

        }

        [HttpPut("EditPost")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {

            string sql = @"UPDATE TutorialAppSchema.Posts  
            SET PostTitle = @postTitle , postContent = @postContent , PostUpdated = GETDATE() 
            WHERE postId = @postId AND UserId = @userId";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            _helper.InsertParametersIntoList(sqlParameters, "@postTitle", SqlDbType.NVarChar, postToEdit.PostTitle);
            _helper.InsertParametersIntoList(sqlParameters, "@postContent", SqlDbType.NVarChar, postToEdit.PostContent);
            _helper.InsertParametersIntoList(sqlParameters, "@postId", SqlDbType.Int, postToEdit.PostId);
            _helper.InsertParametersIntoList(sqlParameters, "@userId", SqlDbType.Int, this.User.FindFirst("userId")?.Value);


            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Did not edit Post correctly");

        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Posts 
            WHERE UserId =" + postId.ToString() +
            "AND UserId = " + this.User.FindFirst("userId")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("did not delete properly from database");
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [postId],
                        [UserId],
                        [PostTitle],
                        [postContent],
                        [PostCreated],
                        [PostUpdated] 
                        FROM TutorialAppSchema.Posts 
                        WHERE PostTitle 
                        LIKE '%" + searchParam + "%' or postContent LIKE '%" + searchParam + "%'";
            return _dapper.LoadData<Post>(sql);
        }


    }

}