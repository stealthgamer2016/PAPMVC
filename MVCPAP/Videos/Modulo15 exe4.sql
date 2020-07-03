
--1
CREATE FUNCTION dbo.udfPrecoFinal(
@preco MONEY,
@iva   Decimal(5,2)
)
RETURNS MONEY
AS
BEGIN
RETURN @preco * @iva;
END

--2
Create Procedure dbo.uspInsertProduto(
@nome			nvarchar(100),
@preco			money,
@iva			decimal(5,2),
@stock			smallint
)
As
Insert into Produto(nome,preco,precoFinal,iva,stock)
Values (@nome,@preco,dbo.udfPrecoFinal(@preco,@iva),@iva,@stock)

--3
CREATE FUNCTION dbo.udfdatapagamento(
@datafactura		Datetime2
)
RETURNS Datetime2
AS
BEGIN
RETURN DateAdd(Day,120,@datafactura);
END

--4
Create Procedure dbo.uspInsertFactura(
@cliente			int,
@DataFactura		Datetime2
)
As
Insert into Factura(cliente,dataEmissao,dataPagamento)
Values (@cliente,@DataFactura,dbo.udfdatapagamento(@DataFactura))

--5
CREATE FUNCTION dbo.udftotalfactura(
@idFactura		int
)
RETURNS Table
AS
Return Select SUM(P.preco)'Preço'
From ProdutoFactura PF
Join Produto P On PF.produto=P.idProduto
Where PF.factura=@idFactura
Group by PF.factura;

--6
CREATE FUNCTION dbo.udflistafacturas(
@idCliente		int
)
RETURNS Table
AS
Return Select *
From Factura
Where cliente=@idCliente;

--7
CREATE FUNCTION dbo.udfprodutosstockinferior(
@valor		int
)
RETURNS Table
AS
Return Select *
From Produto
Where stock<@valor;

--8
CREATE FUNCTION dbo.udffacturapassada(
)
RETURNS Table
AS
Return Select *
From Factura
Where dataPagamento>GETDATE()

--9
CREATE FUNCTION dbo.udfclientefacturapassada(
)
RETURNS Table
AS
Return Select C.nome
From Cliente C
Join Factura F On C.idCliente=F.cliente
Where F.dataPagamento>GETDATE()
Group by C.nome

--10
CREATE FUNCTION dbo.udfvendasemano(
@ano		int
)
RETURNS Table
AS
Return Select count(PF.idFacturaProduto)'Vendas'
From ProdutoFactura PF
Join Factura F On PF.factura=F.idFactura
Where DATEPART(YEAR,F.dataEmissao)=@ano;
