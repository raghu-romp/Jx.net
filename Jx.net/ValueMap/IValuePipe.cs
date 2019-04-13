using System;
using System.Collections.Generic;
using System.Text;

namespace Jx.net
{
    public interface IValuePipe
    {
        string MapperName { get; }
        dynamic Process(dynamic fromValue);
    }
}
