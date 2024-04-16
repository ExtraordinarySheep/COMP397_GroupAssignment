using System;
using System.Collections.Generic;

public interface IObserver
{

    public void OnNotify(SubjectEnums subjectEnum, List<Object> parameters);

}