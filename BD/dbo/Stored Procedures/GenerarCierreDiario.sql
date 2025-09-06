
CREATE   PROCEDURE dbo.GenerarCierreDiario
    @Dia DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ing DECIMAL(18,2) = (
        SELECT SUM(Monto) 
        FROM dbo.Movimiento 
        WHERE Tipo = 'I' AND CAST(Fecha AS DATE) = @Dia
    );

    DECLARE @gas DECIMAL(18,2) = (
        SELECT SUM(Monto) 
        FROM dbo.Movimiento 
        WHERE Tipo = 'G' AND CAST(Fecha AS DATE) = @Dia
    );

    SET @ing = ISNULL(@ing, 0);
    SET @gas = ISNULL(@gas, 0);

    MERGE dbo.CierreDiario AS target
    USING (SELECT @Dia AS Fecha) AS src
    ON target.Fecha = src.Fecha
    WHEN MATCHED THEN
        UPDATE SET 
            TotalIngresos = @ing,
            TotalGastos = @gas,
            SaldoDia = @ing - @gas
    WHEN NOT MATCHED THEN
        INSERT (Fecha, TotalIngresos, TotalGastos, SaldoDia)
        VALUES (@Dia, @ing, @gas, @ing - @gas);
END