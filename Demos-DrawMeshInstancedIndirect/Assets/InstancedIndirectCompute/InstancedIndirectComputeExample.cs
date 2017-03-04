// https://docs.unity3d.com/560/Documentation/ScriptReference/Graphics.DrawMeshInstancedIndirect.html

using UnityEngine;
using UnityEngine.Rendering;

public class InstancedIndirectComputeExample : MonoBehaviour
{ 
    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;

    public ShadowCastingMode castShadows = ShadowCastingMode.Off;
    public bool receiveShadows = false;

    public ComputeShader positionComputeShader;
    private int positionComputeKernelId;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer colorBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
 
    void Start()
	{
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        CreateBuffers();
    }

    void Update()
	{ 
        // Update position buffer
        UpdateBuffers();

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, instanceMesh.bounds, argsBuffer, 0, null, castShadows, receiveShadows);
    }

    void UpdateBuffers()
    {
        /// TODO this only works with POT, integral sqrt vals
        int bs = instanceCount / 64;
        positionComputeShader.Dispatch(positionComputeKernelId, bs, 1, 1);
    }


    void CreateBuffers()
	{ 
		if ( instanceCount < 1 ) instanceCount = 1;

        instanceCount = Mathf.ClosestPowerOfTwo(instanceCount);

        positionComputeKernelId = positionComputeShader.FindKernel("CSPositionKernel");
        instanceMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        // Positions & Colors
        if (positionBuffer != null) positionBuffer.Release();
        if (colorBuffer != null) colorBuffer.Release();

        positionBuffer	= new ComputeBuffer(instanceCount, 16);
        colorBuffer = new ComputeBuffer(instanceCount, 16);

		Vector4[] colors = new Vector4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
            colors[i] = Random.ColorHSV();

        colorBuffer.SetData(colors);

        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);
        instanceMaterial.SetBuffer("colorBuffer", colorBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        positionComputeShader.SetBuffer(positionComputeKernelId, "positionBuffer", positionBuffer);
        positionComputeShader.SetFloat("_Dim", Mathf.Sqrt(instanceCount));
        positionComputeShader.SetFloat("_Time", Time.time);
    }

    void OnDisable()
	{
        if (positionBuffer != null) positionBuffer.Release();
        positionBuffer = null;

        if (colorBuffer != null) colorBuffer.Release();
        colorBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(265, 12, 200, 30), "Instance Count: " + instanceCount.ToString("N0"));
        instanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)instanceCount, 1.0f, 5000000.0f);
    }
}
