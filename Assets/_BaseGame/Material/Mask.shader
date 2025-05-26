
// "Invisible" Unity Occlusion Shader. Useful for AR, Masking, etc
// Mark Johns / Doomlaser - https://twitter.com/Doomlaser

Shader "DepthMask"
{

    SubShader
    {
         Tags {"Queue" = "Transparent+1" }

         Pass{
             Blend Zero One
             }
    }
}