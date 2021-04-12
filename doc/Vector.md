# Struct Vector

Representa um par ordenado de coordenadas que define um vetor ou ponto no espaço bidimensional.

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector

## Construtores

### Vector(float, float)

Inicializa uma nova instância de Vector com base em suas coordenadas.

	public Vector (float x, float y)
    


### Vector(float)

Inicializa uma nova instância de Vector que tenha módulo 1 e a o ângulo dado em graus.

	public Vector (float angle)
    
**Observações**

>O ângulo 0° aponta para o eixo Y enquanto que o ângulo 90° aponta para o eixo X.

## Propriedades

### Vector.Angle

Obtém o ângulo do vetor.

	public float Angle { get; }
    
**Exemplo**

	public void TestVector()
    {
    	Vector vector = new Vector(1, 1);
        Console.WriteLine(vector.Angle); //45
    }

**Observações**

>Caso o vetor tenha módulo zero, o ângulo retornado será sempre zero.



### Vector.Module

Obtém o módulo do vetor.

	public float Module { get; }



### Vector.X e Vector.Y

Obtém uma coordenada do vetor.

	public float X { get; }
	public float Y { get; }
    
### Vector.Origin

Obtém uma instância do vetor central (0, 0).

	public static Vector Origin { get; }

## Métodos

### Vector.FromAngle(float)

Mesmo que new Vector(float).

	public static Vector FromAngle(float angle)


### Vector.FromAngle(float, float)

Obtém uma instância de Vector dados ângulo e módulo.

	public static Vector FromAngle(float angle, float module)

### Vector.GetDistance(Vector)

Obtém a distância entre este e outro vetor.

	public float GetDistance(Vector vector)

### Vector.ToString()

Obtém uma string formatada para representar este vetor.

	public override string ToString()
    
**Exemplo**

	public void ShowVector()
    {
    	Vector vector = new Vector(1, 1);
        Console.WriteLine(vector);
    }

### Vector.GetPolygon(Vector, float, int)

Obtém uma matriz de vetores que compoem os vértices de um polígono regular dados centro, 

	public static Vector[] GetPolygon(Vector center, float radius, int edges)
    
**Exemplo**

	public void CreatePolygon()
    {
    	Vector[] verts = Vector.GetPolygon(
        		Vector.Origin,
                1.0f,
                7);
    }

## Operadores

### Vector.+(Vector, Vector)

	public static Vector operator +(Vector left, Vector right)

### Vector.-(Vector, Vector)

	public static Vector operator -(Vector left, Vector right)

### Vector.\*(Vector, Vector)

Obtém o produto vetorial no plano bidimensional entre os dois vetores.

	public static Vector operator *(Vector left, Vector right)

### Vector.\*(Vector, float) e Vector.\*(float, Vector)

Obtém o resultado da multiplicação de um vetor por ume scalar. O Vetor resultante terá mesma direção que o inicial e seu módulo será multiplicado pelo escalar.

	public static Vector operator *(Vector vector, float scalar)
	public static Vector operator *(float scalar, Vector vector)
    
### Vector./(Vector, float)

Obtém a divisão de um vetor por um escalar. Em outras palavras, a multiplicação do mesmo pelo inverso do escalar.

	public static Vector operator /(Vector vector, float scalar)