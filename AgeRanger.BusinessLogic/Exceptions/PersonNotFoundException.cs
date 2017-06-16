﻿using System;

namespace AgeRanger.BusinessLogic.Exceptions
{
    public class PersonNotFoundException: Exception
    {
        public PersonNotFoundException(string message)
            : base(message)
        { }
    }
}
