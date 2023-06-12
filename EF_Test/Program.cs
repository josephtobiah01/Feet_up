// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using ParentMiddleWare;
using UserApi.Net7;

Console.WriteLine("Hello, World!");

MiddleWare.SetBaseUrl("https://localhost:7174");


//var user = await UserMiddleware.Instance.GetUserByID(4);

//Console.WriteLine(JsonConvert.SerializeObject(user));

//await UserMiddleware.Instance.CreateUser(new User() { FirstName = "MOO" });


//using (var ctx = new UserContext())
//{

    //try
    //{
    //    var obj = ctx.Users.Where(t => t.FirstName == "Bill").FirstOrDefault();

    //    var objmess = obj.Messages;

    //    var objuser = obj.ConnectedUsers;


    //}
    //catch { }

    //var stud = new User() { FirstName = "Bill" };

    //var mess = new Message { PlaceHolder = "dsdsds" };
    //var mess2 = new Message { PlaceHolder = "dsds3ds" };
    //var mess3 = new Message { PlaceHolder = "dsd4343s3ds" };


    //stud.Messages.Add(mess);
    //stud.Messages.Add(mess2);
    //stud.Messages.Add(mess3);

    //var stud2 = new ConnectedUsers() { ParentUser = stud, User = new User() { FirstName = "c1" } };
    //var stud3 = new ConnectedUsers() { ParentUser = stud, User = new User() { FirstName = "c2" } };
    //var stud4 = new ConnectedUsers() { ParentUser = stud, User = new User() { FirstName = "c3" } };
    //var stud5 = new ConnectedUsers() { ParentUser = stud, User = new User() { FirstName = "c4" } };



    //stud.ConnectedUsers.Add(stud2);
    //stud.ConnectedUsers.Add(stud3);
    //stud.ConnectedUsers.Add(stud4);
    //stud.ConnectedUsers.Add(stud5);


    //ctx.Users.Add(stud);
  //  ctx.SaveChanges();
//}
