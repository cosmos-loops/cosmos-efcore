namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    public interface ISetFieldSqlGenerator
    {
        string TranslateToSql(SetFieldInfo operation);
    }
}