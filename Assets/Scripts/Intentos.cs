using Postgrest.Models;
using Postgrest.Attributes;

public class intentos : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("puntaje")]
    public int puntaje { get; set; }

    [Column("tiempo")]
    public int tiempo { get; set; }

    [Column("resp_correct")]
    public int resp_correct { get; set; }

    [Column("id_usuario")]
    public int id_usuario { get; set; }

    [Column("id_categoria")]
    public int id_categoria { get; set; }
}