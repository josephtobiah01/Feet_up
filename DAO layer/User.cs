//using DAO_layer.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DAO_layer
//{
//    [Serializable]
//    public class User
//    {
//        public long ID { get; set; }
//        public string? FirstName { get; set; }
//        public string? LastName { get; set; }
//        public string? Email { get; set; }

//        public UserTiers Level { get; set; }

//        public User()
//        {
//            this.Messages = new List<Message>();
//            this.ConnectedUsers = new List<ConnectedUsers>();
//        }

//        public UserCategory UserCategory
//        {
//            get
//            {
//                if ((int)Level < 1000) return UserCategory.USER;
//                if ((int)Level < 99999) return UserCategory.TRAINER;
//                return UserCategory.BACKOFFICE;
//            }
//        }

//        // Contents of this users Message history

//        // Virtual so it's lazy-loading, only gets loaded when accessed
//        public virtual ICollection<Message> Messages { get; set; }


//        // Connected Users, relationship determined by the Levels of respective user
//        // i.e if .this.UserCategory is a User, then that's their trainer(s) and supervisor(s)
//        // if .this.Usercategory is Trainer or Backoffice, then this is their UserList

//        // Use GetRelationship function to determine

//        // Virtual so it's lazy-loading, only gets loaded when accessed

//        public virtual ICollection<ConnectedUsers> ConnectedUsers { get; set; }



//        // Funtions


//        // returns the relationship this user has to the supplied user. It's from the perspective of .THIS user
//        public UserCategory GetRelationship(ref User user)
//        {
//            if (this.UserCategory == UserCategory.USER)
//            {
//                if (user.UserCategory == UserCategory.USER) return UserCategory.ERROR;
//                if (user.UserCategory == UserCategory.TRAINER) return UserCategory.TRAINER;
//                if (user.UserCategory == UserCategory.BACKOFFICE) return UserCategory.BACKOFFICE;
//            }

//            if (this.UserCategory == UserCategory.TRAINER)
//            {
//                if (user.UserCategory == UserCategory.USER) return UserCategory.USER;
//                if (user.UserCategory == UserCategory.TRAINER) return UserCategory.ERROR;
//                if (user.UserCategory == UserCategory.BACKOFFICE) return UserCategory.BACKOFFICE;
//            }

//            if (this.UserCategory == UserCategory.BACKOFFICE)
//            {
//                if (user.UserCategory == UserCategory.USER) return UserCategory.USER;
//                if (user.UserCategory == UserCategory.TRAINER) return UserCategory.TRAINER;
//                if (user.UserCategory == UserCategory.BACKOFFICE) return UserCategory.BACKOFFICE;
//            }

//            return UserCategory.ERROR;
//        }


//        // Connect a new User to this user
//        public void AddUserConnection(User user)
//        {
//        //    this.ConnectedUsers.Users.Add(user);
//        }

//        // removes a user from the user connection
//        public void DeleteUserConnecction(User user)
//        {
//        //    this.ConnectedUsers.Users.Remove(user);
//        }

//        public static User FindUserByID(long ID)
//        {
//            using (var ctx = new UserContext())
//            {
//                return ctx.Users.Where(t => t.ID == ID).FirstOrDefault();
//            }
//        }

//        public void SetUserLevel(UserTiers Level)
//        {
//            using (var ctx = new UserContext())
//            {
//                this.Level = Level;
//                ctx.SaveChanges();
//            }
//        }

//        public void DeleteUser(User User)
//        {
//            using (var ctx = new UserContext())
//            {
//                ctx.Users.Remove(User);
//                ctx.SaveChanges();
//            }
//        }

//        public void AddUser(User User)
//        {
//            using (var ctx = new UserContext())
//            {
//                ctx.Users.Add(User);
//                ctx.SaveChanges();
//            }
//        }
//    }
//}