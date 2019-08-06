namespace Pomelo.EntityFrameworkCore.Lolita.Update
{
    /// <summary>
    /// Interface for set field sql generator
    /// </summary>
    public interface ISetFieldSqlGenerator
    {
        /// <summary>
        /// Translate to sql
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        string TranslateToSql(SetFieldInfo operation);
    }
}