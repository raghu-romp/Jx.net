using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public interface IValuePipe
    {
        string Name { get; }
        dynamic Process(dynamic fromValue);
    }
}
