namespace MarcasAutos.Domain.MarcasAutos;

public class MarcaAuto
{
  public int Id { get; private set; }

  public string Nombre { get; private set; } = string.Empty;

  private MarcaAuto()
  {
    // Constructor requerido por Entity Framework.
  }

  public MarcaAuto(string nombre)
  {
    SetNombre(nombre);
  }

  public void ActualizarNombre(string nombre)
  {
    SetNombre(nombre);
  }

  private void SetNombre(string nombre)
  {
    if (string.IsNullOrWhiteSpace(nombre))
    {
      throw new ArgumentException("El nombre de la marca es requerido.", nameof(nombre));
    }

    Nombre = nombre.Trim();
  }
}
