# DrawMeshInstanceIndirect examples.
Exploring the Unity 5.6 DrawMeshInstanceIndirect method to render large numbers of objects.

### InstancedIndirectExample
The example on scene InstancedIndirectExample is a slightly expanded version of [Unity's sample](https://docs.unity3d.com/560/Documentation/ScriptReference/Graphics.DrawMeshInstancedIndirect.html) code provided by [noisecrime](https://github.com/noisecrime/Unity-InstancedIndirectExamples). The buffer is created in CPU and the position sampled in GPU. This approach is useful when the positions are static, otherwise the cost of update from CPU is very high.

### InstancedIndirectComputeExample
The example on scene InstancedIndirectComputeExample demonstrates the use of Compute Shaders to generate position data. Compute Shaders are specially useful when the buffers need constant update, as the buffer remains in GPU and we can take advantage of many threads to perform the calculations. 


### InstancedIndirectNoBuffer
The example on scene InstancedIndirectNoBuffer shows how to position the objects on the fly, direclty within the shader. This approach eliminates the use of any auxiliar buffer, and positions can be calculated directly in the surface shader. This is very attractive for when the calculations are simple and the number of instances is very high.

** NOTE: these are WIP demos.

