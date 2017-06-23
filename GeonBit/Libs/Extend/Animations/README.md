
# GeonBit.Extend.Animation.*

Play animated 3D models and support for CPU animation.
CPU animation is optimized using unsafe code, writing directly to mapped VertexBuffer memory using reflection (DirectX) and unmanaged/C++ code (WP8.0). 

## Importers

* 'Animation' - Import animations from a Model.
* 'GPU AnimatedModel' - Import an animated Model.
* 'CPU AnimatedModel' - Import an animated Model to be animated by the CPU. Based on DynamicModelProcessor, the imported asset is of type Microsoft.Xna.Framework.Graphics.Model where the VertexBuffer is replaced by a CpuAnimatedVertexBuffer. CpuAnimatedVertexBuffer inherits from DynamicVertexBuffer.

## Example

-Import 3D model with GPU AnimatedModel or GPU AnimatedModel Processor. Use SkinnedEffect for GPU and BasicEffect for CPU based animation.

-Load as any 3D Model:

	_model = Content.Load<Model>("animatedModel");

-Load the Animations from model:

	_animations = _model.GetAnimations();
	var clip = _animations.Clips["ClipName"];
        _animations.SetClip(clip);

-Update animation on every frame:

        _animations.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

-Draw GPU animation:

	foreach (ModelMesh mesh in _model.Meshes)
	{
		foreach (var meshPart in mesh.MeshParts)
		{
			meshPart.effect.SetBoneTransforms(_animations.AnimationTransforms);
			// set effect parameters, lights, etc          
		}
		mesh.Draw();
	}

-Draw CPU animation:

	foreach (ModelMesh mesh in _model.Meshes)
	{
		foreach (var meshPart in mesh.MeshParts)
		{
		       meshPart.UpdateVertices(animationPlayer.AnimationTransforms);
		       // set effect parameters, lights, etc
		}
		mesh.Draw();
	}




