using System;

namespace AgeRanger.BusinessLogic.Exceptions
{
    public class AgeGroupsEmptyException: Exception
    {
        public AgeGroupsEmptyException(string message) 
            : base(message)
        { }
    }
}
