﻿#region	License
//--------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Elect </Project>
//     <File>
//         <Name> IUnitOfWorkTransaction.cs </Name>
//         <Created> 25/03/2018 10:04:53 PM </Created>
//         <Key> 783f5f69-237c-4bb3-8b30-f3add5a6b446 </Key>
//     </File>
//     <Summary>
//         IUnitOfWorkTransaction.cs is a part of Elect
//     </Summary>
// <License>
//--------------------------------------------------
#endregion License

using System;

namespace Elect.Data.EF.Interfaces.UnitOfWork
{
    public interface IUnitOfWorkTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}