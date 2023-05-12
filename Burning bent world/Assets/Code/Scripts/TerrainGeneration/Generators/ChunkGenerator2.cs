namespace TerrainGeneration.Generators
{
    public class ChunkGenerator2
    {
         private void func_147423_a(int p_147423_1_, int p_147423_2_, int p_147423_3_)
        {
            // double var4 = 684.412D;
            // double var6 = 684.412D;
            // double var8 = 512.0D;
            // double var10 = 512.0D;
            // this.field_147426_g = this.noiseGen6.generateNoiseOctaves(this.field_147426_g, p_147423_1_, p_147423_3_, 5, 5, 200.0D, 200.0D, 0.5D);
            // this.field_147427_d = this.field_147429_l.generateNoiseOctaves(this.field_147427_d, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 8.555150000000001D, 4.277575000000001D, 8.555150000000001D);
            // this.field_147428_e = this.field_147431_j.generateNoiseOctaves(this.field_147428_e, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 684.412D, 684.412D, 684.412D);
            // this.field_147425_f = this.field_147432_k.generateNoiseOctaves(this.field_147425_f, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 684.412D, 684.412D, 684.412D);
            // boolean var45 = false;
            // boolean var44 = false;
            // int var12 = 0;
            // int var13 = 0;
            // double var14 = 8.5D;
            //
            // for (int var16 = 0; var16 < 5; ++var16)
            // {
            //     for (int var17 = 0; var17 < 5; ++var17)
            //     {
            //         float var18 = 0.0F;
            //         float var19 = 0.0F;
            //         float var20 = 0.0F;
            //         byte var21 = 2;
            //         BiomeGenBase var22 = this.biomesForGeneration[var16 + 2 + (var17 + 2) * 10];
            //
            //         for (int var23 = -var21; var23 <= var21; ++var23)
            //         {
            //             for (int var24 = -var21; var24 <= var21; ++var24)
            //             {
            //                 BiomeGenBase var25 = this.biomesForGeneration[var16 + var23 + 2 + (var17 + var24 + 2) * 10];
            //                 float var26 = var25.minHeight;
            //                 float var27 = var25.maxHeight;
            //
            //                 if (this.field_147435_p == WorldType.field_151360_e && var26 > 0.0F)
            //                 {
            //                     var26 = 1.0F + var26 * 2.0F;
            //                     var27 = 1.0F + var27 * 4.0F;
            //                 }
            //
            //                 float var28 = this.parabolicField[var23 + 2 + (var24 + 2) * 5] / (var26 + 2.0F);
            //
            //                 if (var25.minHeight > var22.minHeight)
            //                 {
            //                     var28 /= 2.0F;
            //                 }
            //
            //                 var18 += var27 * var28;
            //                 var19 += var26 * var28;
            //                 var20 += var28;
            //             }
            //         }
            //
            //         var18 /= var20;
            //         var19 /= var20;
            //         var18 = var18 * 0.9F + 0.1F;
            //         var19 = (var19 * 4.0F - 1.0F) / 8.0F;
            //         double var46 = this.field_147426_g[var13] / 8000.0D;
            //
            //         if (var46 < 0.0D)
            //         {
            //             var46 = -var46 * 0.3D;
            //         }
            //
            //         var46 = var46 * 3.0D - 2.0D;
            //
            //         if (var46 < 0.0D)
            //         {
            //             var46 /= 2.0D;
            //
            //             if (var46 < -1.0D)
            //             {
            //                 var46 = -1.0D;
            //             }
            //
            //             var46 /= 1.4D;
            //             var46 /= 2.0D;
            //         }
            //         else
            //         {
            //             if (var46 > 1.0D)
            //             {
            //                 var46 = 1.0D;
            //             }
            //
            //             var46 /= 8.0D;
            //         }
            //
            //         ++var13;
            //         double var47 = (double)var19;
            //         double var48 = (double)var18;
            //         var47 += var46 * 0.2D;
            //         var47 = var47 * 8.5D / 8.0D;
            //         double var29 = 8.5D + var47 * 4.0D;
            //
            //         for (int var31 = 0; var31 < 33; ++var31)
            //         {
            //             double var32 = ((double)var31 - var29) * 12.0D * 128.0D / 256.0D / var48;
            //
            //             if (var32 < 0.0D)
            //             {
            //                 var32 *= 4.0D;
            //             }
            //
            //             double var34 = this.field_147428_e[var12] / 512.0D;
            //             double var36 = this.field_147425_f[var12] / 512.0D;
            //             double var38 = (this.field_147427_d[var12] / 10.0D + 1.0D) / 2.0D;
            //             double var40 = MathHelper.denormalizeClamp(var34, var36, var38) - var32;
            //
            //             if (var31 > 29)
            //             {
            //                 double var42 = (double)((float)(var31 - 29) / 3.0F);
            //                 var40 = var40 * (1.0D - var42) + -10.0D * var42;
            //             }
            //
            //             this.field_147434_q[var12] = var40;
            //             ++var12;
            //         }
            //     }
            // }
        }
    }
}