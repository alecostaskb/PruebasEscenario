public enum tiposTerreno
{
    Normal = 0,
    Obstructivo = 2,
    Prohibido = 100
}

public enum lateralesCelda
{
    Arriba = 0,
    Derecha = 1,
    Abajo = 2,
    Izquierda = 3
}

public enum tiposLateralCelda
{
    Nada = 0,
    Pared = 1,
    PuertaAbierta = 2,
    PuertaCerrada = 3
}

public class InfoCelda
{
    // posición en el grid
    public int x;

    public int y;

    // tipo de terreno - por defecto es normal
    public int tipoTerreno = (int)tiposTerreno.Normal;

    // lateral y tipo de lateral (hay una pared, puerta, ...)

    // lateral:
    // Arriba = 0,
    // Derecha = 1,
    // Abajo = 2,
    // Izquierda = 3

    // tipo de lateral:
    // Nada = 0,
    // Pared = 1,
    // PuertaAbierta = 2,
    // PuertaCerrada = 3
    public InfoLateralCelda[] laterales = new InfoLateralCelda[]
    {
        new InfoLateralCelda(lateralesCelda.Arriba, tiposLateralCelda.Nada),
        new InfoLateralCelda(lateralesCelda.Derecha, tiposLateralCelda.Nada),
        new InfoLateralCelda(lateralesCelda.Abajo, tiposLateralCelda.Nada),
        new InfoLateralCelda(lateralesCelda.Izquierda, tiposLateralCelda.Nada)
    };

    // indica si la celda está en una zona cerrada
    public bool estaEnZonaCerrada = false;
}

public class InfoLateralCelda
{
    // el lateral de la celda a la que se le pondrá el tipo de lateral
    // Arriba = 0,
    // Derecha = 1,
    // Abajo = 2,
    // Izquierda = 3
    public lateralesCelda lateralCelda;

    // el tipo de lateral que tiene el lateral de la celda
    // Nada = 0,
    // Pared = 1,
    // PuertaAbierta = 2,
    // PuertaCerrada = 3
    public tiposLateralCelda tipoLateralCelda;

    public InfoLateralCelda(lateralesCelda lateralCelda, tiposLateralCelda tipoLateralCelda)
    {
        this.lateralCelda = lateralCelda;
        this.tipoLateralCelda = tipoLateralCelda;
    }
}