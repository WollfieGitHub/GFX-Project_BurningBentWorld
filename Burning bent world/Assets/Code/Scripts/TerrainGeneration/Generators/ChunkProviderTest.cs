﻿using System;

namespace TerrainGeneration.Generators
{
    public class ChunkProviderTest
    {
        // /** RNG. */
        // private Random rand;
        // private NoiseGeneratorOctaves noise_x;
        // private NoiseGeneratorOctaves noise_z;
        // private NoiseGeneratorOctaves noise_y;
        // private NoiseGeneratorPerlin noise_mixer;
        //
        // /** A NoiseGeneratorOctaves used in generating terrain */
        // public NoiseGeneratorOctaves noiseGen5;
        //
        // /** A NoiseGeneratorOctaves used in generating terrain */
        // public NoiseGeneratorOctaves noiseGen6;
        // public NoiseGeneratorOctaves mobSpawnerNoise;
        //
        // /** Reference to the World object. */
        // private World worldObj;
        //
        // /** are map structures going to be generated (e.g. strongholds) */
        // private readonly bool mapFeaturesEnabled;
        // private WorldType world_type;
        // private readonly double[] field_147434_q;
        // private readonly float[] parabolicField;
        // private double[] stoneNoise = new double[256];
        // private MapGenBase caveGenerator = new MapGenCaves();
        //
        // /** Holds Stronghold Generator */
        // private MapGenStronghold strongholdGenerator = new MapGenStronghold();
        //
        // /** Holds Village Generator */
        // private MapGenVillage villageGenerator = new MapGenVillage();
        //
        // /** Holds Mineshaft Generator */
        // private MapGenMineshaft mineshaftGenerator = new MapGenMineshaft();
        // private MapGenScatteredFeature scatteredFeatureGenerator = new MapGenScatteredFeature();
        //
        // /** Holds ravine generator */
        // private MapGenBase ravineGenerator = new MapGenRavine();
        //
        // /** The biomes that are used to generate the chunk */
        // private BiomeGenBase[] biomesForGeneration;
        // double[] field_147427_d;
        // double[] field_147428_e;
        // double[] field_147425_f;
        // double[] field_147426_g;
        // int[][] field_73219_j = new int[32][32];
        //
        // public ChunkProviderGenerate(World world, long seed, boolean featuresEnabled)
        // {
        //     worldObj = world;
        //     mapFeaturesEnabled = featuresEnabled;
        //     world_type = world.getWorldInfo().getTerrainType();
        //     rand = new Random(seed);
        //     noise_x = new NoiseGeneratorOctaves(rand, 16);
        //     noise_z = new NoiseGeneratorOctaves(rand, 16);
        //     noise_y = new NoiseGeneratorOctaves(rand, 8);
        //     noise_mixer = new NoiseGeneratorPerlin(rand, 4);
        //     noiseGen5 = new NoiseGeneratorOctaves(rand, 10);
        //     noiseGen6 = new NoiseGeneratorOctaves(rand, 16);
        //     mobSpawnerNoise = new NoiseGeneratorOctaves(rand, 8);
        //     field_147434_q = new double[825];
        //     parabolicField = new float[25];
        //
        //     for (int x = -2; x <= 2; ++x)
        //     {
        //         for (int z = -2; z <= 2; ++z)
        //         {
        //             float var7 = 10.0F / MathF.Sqrt((x * x + z * z) + 0.2F);
        //             parabolicField[x + 2 + (z + 2) * 5] = var7;
        //         }
        //     }
        // }
        //
        // public void func_147424_a(int blockX, int blockZ, Block[] p_147424_3_)
        // {
        //     byte var4 = 63;
        //     biomesForGeneration = worldObj.getWorldChunkManager().getBiomesForGeneration(
        //         biomesForGeneration, 
        //         blockX * 4 - 2,
        //         blockZ * 4 - 2,
        //         10, 10
        //     );
        //     func_147423_a(blockX * 4, 0, blockZ * 4);
        //
        //     for (int blockX = 0; blockX < 4; ++blockX)
        //     {
        //         int var6 = blockX * 5;
        //         int var7 = (blockX + 1) * 5;
        //
        //         for (int blockZ = 0; blockZ < 4; ++blockZ)
        //         {
        //             int var9 = (var6 + blockZ) * 33;
        //             int var10 = (var6 + blockZ + 1) * 33;
        //             int var11 = (var7 + blockZ) * 33;
        //             int var12 = (var7 + blockZ + 1) * 33;
        //
        //             for (int var13 = 0; var13 < 32; ++var13)
        //             {
        //                 double var14 = 0.125D;
        //                 double var16 = field_147434_q[var9 + var13];
        //                 double var18 = field_147434_q[var10 + var13];
        //                 double var20 = field_147434_q[var11 + var13];
        //                 double var22 = field_147434_q[var12 + var13];
        //                 double var24 = (field_147434_q[var9 + var13 + 1] - var16) * var14;
        //                 double var26 = (field_147434_q[var10 + var13 + 1] - var18) * var14;
        //                 double var28 = (field_147434_q[var11 + var13 + 1] - var20) * var14;
        //                 double var30 = (field_147434_q[var12 + var13 + 1] - var22) * var14;
        //
        //                 for (int var32 = 0; var32 < 8; ++var32)
        //                 {
        //                     double var33 = 0.25D;
        //                     double var35 = var16;
        //                     double var37 = var18;
        //                     double var39 = (var20 - var16) * var33;
        //                     double var41 = (var22 - var18) * var33;
        //
        //                     for (int var43 = 0; var43 < 4; ++var43)
        //                     {
        //                         int var44 = var43 + blockX * 4 << 12 | 0 + blockZ * 4 << 8 | var13 * 8 + var32;
        //                         short var45 = 256;
        //                         var44 -= var45;
        //                         double var46 = 0.25D;
        //                         double var50 = (var37 - var35) * var46;
        //                         double var48 = var35 - var50;
        //
        //                         for (int var52 = 0; var52 < 4; ++var52)
        //                         {
        //                             if ((var48 += var50) > 0.0D)
        //                             {
        //                                 p_147424_3_[var44 += var45] = Blocks.stone;
        //                             }
        //                             else if (var13 * 8 + var32 < var4)
        //                             {
        //                                 p_147424_3_[var44 += var45] = Blocks.water;
        //                             }
        //                             else
        //                             {
        //                                 p_147424_3_[var44 += var45] = null;
        //                             }
        //                         }
        //
        //                         var35 += var39;
        //                         var37 += var41;
        //                     }
        //
        //                     var16 += var24;
        //                     var18 += var26;
        //                     var20 += var28;
        //                     var22 += var30;
        //                 }
        //             }
        //         }
        //     }
        // }
        //
        // public void func_147422_a(int p_147422_1_, int p_147422_2_, Block[] p_147422_3_, byte[] p_147422_4_, BiomeGenBase[] p_147422_5_)
        // {
        //     double var6 = 0.03125D;
        //     stoneNoise = noise_mixer.func_151599_a(stoneNoise, (double)(p_147422_1_ * 16), (double)(p_147422_2_ * 16), 16, 16, var6 * 2.0D, var6 * 2.0D, 1.0D);
        //
        //     for (int var8 = 0; var8 < 16; ++var8)
        //     {
        //         for (int var9 = 0; var9 < 16; ++var9)
        //         {
        //             BiomeGenBase var10 = p_147422_5_[var9 + var8 * 16];
        //             var10.func_150573_a(worldObj, rand, p_147422_3_, p_147422_4_, p_147422_1_ * 16 + var8, p_147422_2_ * 16 + var9, stoneNoise[var9 + var8 * 16]);
        //         }
        //     }
        // }
        //
        // /**
        //  * loads or generates the chunk at the chunk location specified
        //  */
        // public Chunk loadChunk(int p_73158_1_, int p_73158_2_)
        // {
        //     return provideChunk(p_73158_1_, p_73158_2_);
        // }
        //
        // /**
        //  * Will return back a chunk, if it doesn't exist and its not a MP client it will generates all the blocks for the
        //  * specified chunk from the map seed and chunk seed
        //  */
        // public Chunk provideChunk(int p_73154_1_, int p_73154_2_)
        // {
        //     rand.setSeed((long)p_73154_1_ * 341873128712L + (long)p_73154_2_ * 132897987541L);
        //     Block[] var3 = new Block[65536];
        //     byte[] var4 = new byte[65536];
        //     func_147424_a(p_73154_1_, p_73154_2_, var3);
        //     biomesForGeneration = worldObj.getWorldChunkManager().loadBlockGeneratorData(biomesForGeneration, p_73154_1_ * 16, p_73154_2_ * 16, 16, 16);
        //     func_147422_a(p_73154_1_, p_73154_2_, var3, var4, biomesForGeneration);
        //     caveGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //     ravineGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //
        //     if (mapFeaturesEnabled)
        //     {
        //         mineshaftGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //         villageGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //         strongholdGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //         scatteredFeatureGenerator.func_151539_a(this, worldObj, p_73154_1_, p_73154_2_, var3);
        //     }
        //
        //     Chunk var5 = new Chunk(worldObj, var3, var4, p_73154_1_, p_73154_2_);
        //     byte[] var6 = var5.getBiomeArray();
        //
        //     for (int var7 = 0; var7 < var6.length; ++var7)
        //     {
        //         var6[var7] = (byte)biomesForGeneration[var7].biomeID;
        //     }
        //
        //     var5.generateSkylightMap();
        //     return var5;
        // }
        //
        // private void func_147423_a(int p_147423_1_, int p_147423_2_, int p_147423_3_)
        // {
        //     double var4 = 684.412D;
        //     double var6 = 684.412D;
        //     double var8 = 512.0D;
        //     double var10 = 512.0D;
        //     field_147426_g = noiseGen6.generateNoiseOctaves(field_147426_g, p_147423_1_, p_147423_3_, 5, 5, 200.0D, 200.0D, 0.5D);
        //     field_147427_d = noise_y.generateNoiseOctaves(field_147427_d, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 8.555150000000001D, 4.277575000000001D, 8.555150000000001D);
        //     field_147428_e = noise_x.generateNoiseOctaves(field_147428_e, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 684.412D, 684.412D, 684.412D);
        //     field_147425_f = noise_z.generateNoiseOctaves(field_147425_f, p_147423_1_, p_147423_2_, p_147423_3_, 5, 33, 5, 684.412D, 684.412D, 684.412D);
        //     boolean var45 = false;
        //     boolean var44 = false;
        //     int var12 = 0;
        //     int var13 = 0;
        //     double var14 = 8.5D;
        //
        //     for (int var16 = 0; var16 < 5; ++var16)
        //     {
        //         for (int var17 = 0; var17 < 5; ++var17)
        //         {
        //             float var18 = 0.0F;
        //             float var19 = 0.0F;
        //             float var20 = 0.0F;
        //             byte var21 = 2;
        //             BiomeGenBase var22 = biomesForGeneration[var16 + 2 + (var17 + 2) * 10];
        //
        //             for (int var23 = -var21; var23 <= var21; ++var23)
        //             {
        //                 for (int var24 = -var21; var24 <= var21; ++var24)
        //                 {
        //                     BiomeGenBase biome = biomesForGeneration[var16 + var23 + 2 + (var17 + var24 + 2) * 10];
        //                     float var26 = biome.minHeight;
        //                     float var27 = biome.maxHeight;
        //
        //                     if (world_type == WorldType.field_151360_e && var26 > 0.0F)
        //                     {
        //                         var26 = 1.0F + var26 * 2.0F;
        //                         var27 = 1.0F + var27 * 4.0F;
        //                     }
        //
        //                     float var28 = parabolicField[var23 + 2 + (var24 + 2) * 5] / (var26 + 2.0F);
        //
        //                     if (biome.minHeight > var22.minHeight)
        //                     {
        //                         var28 /= 2.0F;
        //                     }
        //
        //                     var18 += var27 * var28;
        //                     var19 += var26 * var28;
        //                     var20 += var28;
        //                 }
        //             }
        //
        //             var18 /= var20;
        //             var19 /= var20;
        //             var18 = var18 * 0.9F + 0.1F;
        //             var19 = (var19 * 4.0F - 1.0F) / 8.0F;
        //             double var46 = field_147426_g[var13] / 8000.0D;
        //
        //             if (var46 < 0.0D)
        //             {
        //                 var46 = -var46 * 0.3D;
        //             }
        //
        //             var46 = var46 * 3.0D - 2.0D;
        //
        //             if (var46 < 0.0D)
        //             {
        //                 var46 /= 2.0D;
        //
        //                 if (var46 < -1.0D)
        //                 {
        //                     var46 = -1.0D;
        //                 }
        //
        //                 var46 /= 1.4D;
        //                 var46 /= 2.0D;
        //             }
        //             else
        //             {
        //                 if (var46 > 1.0D)
        //                 {
        //                     var46 = 1.0D;
        //                 }
        //
        //                 var46 /= 8.0D;
        //             }
        //
        //             ++var13;
        //             double var47 = (double)var19;
        //             double var48 = (double)var18;
        //             var47 += var46 * 0.2D;
        //             var47 = var47 * 8.5D / 8.0D;
        //             double var29 = 8.5D + var47 * 4.0D;
        //
        //             for (int var31 = 0; var31 < 33; ++var31)
        //             {
        //                 double var32 = ((double)var31 - var29) * 12.0D * 128.0D / 256.0D / var48;
        //
        //                 if (var32 < 0.0D)
        //                 {
        //                     var32 *= 4.0D;
        //                 }
        //
        //                 double var34 = field_147428_e[var12] / 512.0D;
        //                 double var36 = field_147425_f[var12] / 512.0D;
        //                 double var38 = (field_147427_d[var12] / 10.0D + 1.0D) / 2.0D;
        //                 double var40 = MathHelper.denormalizeClamp(var34, var36, var38) - var32;
        //
        //                 if (var31 > 29)
        //                 {
        //                     double var42 = (double)((float)(var31 - 29) / 3.0F);
        //                     var40 = var40 * (1.0D - var42) + -10.0D * var42;
        //                 }
        //
        //                 field_147434_q[var12] = var40;
        //                 ++var12;
        //             }
        //         }
        //     }
        // }
        //
        // /**
        //  * Checks to see if a chunk exists at x, z
        //  */
        // public bool ChunkExists(int x, int z) => true;
        //
        // /**
        //  * Populates chunk with ores etc etc
        //  */
        // public void Populate(IChunkProvider chunkProvider, int x, int z)
        // {
        //     BlockFalling.field_149832_M = true;
        //     int var4 = x * 16;
        //     int var5 = z * 16;
        //     BiomeGenBase var6 = worldObj.getBiomeGenForCoords(var4 + 16, var5 + 16);
        //     rand.setSeed(worldObj.getSeed());
        //     long var7 = rand.nextLong() / 2L * 2L + 1L;
        //     long var9 = rand.nextLong() / 2L * 2L + 1L;
        //     rand.setSeed((long)x * var7 + (long)z * var9 ^ worldObj.getSeed());
        //     boolean var11 = false;
        //
        //     if (mapFeaturesEnabled)
        //     {
        //         mineshaftGenerator.generateStructuresInChunk(worldObj, rand, x, z);
        //         var11 = villageGenerator.generateStructuresInChunk(worldObj, rand, x, z);
        //         strongholdGenerator.generateStructuresInChunk(worldObj, rand, x, z);
        //         scatteredFeatureGenerator.generateStructuresInChunk(worldObj, rand, x, z);
        //     }
        //
        //     int var12;
        //     int var13;
        //     int var14;
        //
        //     if (var6 != BiomeGenBase.desert && var6 != BiomeGenBase.desertHills && !var11 && rand.nextInt(4) == 0)
        //     {
        //         var12 = var4 + rand.nextInt(16) + 8;
        //         var13 = rand.nextInt(256);
        //         var14 = var5 + rand.nextInt(16) + 8;
        //         (new WorldGenLakes(Blocks.water)).generate(worldObj, rand, var12, var13, var14);
        //     }
        //
        //     if (!var11 && rand.nextInt(8) == 0)
        //     {
        //         var12 = var4 + rand.nextInt(16) + 8;
        //         var13 = rand.nextInt(rand.nextInt(248) + 8);
        //         var14 = var5 + rand.nextInt(16) + 8;
        //
        //         if (var13 < 63 || rand.nextInt(10) == 0)
        //         {
        //             (new WorldGenLakes(Blocks.lava)).generate(worldObj, rand, var12, var13, var14);
        //         }
        //     }
        //
        //     for (var12 = 0; var12 < 8; ++var12)
        //     {
        //         var13 = var4 + rand.nextInt(16) + 8;
        //         var14 = rand.nextInt(256);
        //         int var15 = var5 + rand.nextInt(16) + 8;
        //         (new WorldGenDungeons()).generate(worldObj, rand, var13, var14, var15);
        //     }
        //
        //     var6.decorate(worldObj, rand, var4, var5);
        //     SpawnerAnimals.performWorldGenSpawning(worldObj, var6, var4 + 8, var5 + 8, 16, 16, rand);
        //     var4 += 8;
        //     var5 += 8;
        //
        //     for (var12 = 0; var12 < 16; ++var12)
        //     {
        //         for (var13 = 0; var13 < 16; ++var13)
        //         {
        //             var14 = worldObj.getPrecipitationHeight(var4 + var12, var5 + var13);
        //
        //             if (worldObj.isBlockFreezable(var12 + var4, var14 - 1, var13 + var5))
        //             {
        //                 worldObj.setBlock(var12 + var4, var14 - 1, var13 + var5, Blocks.ice, 0, 2);
        //             }
        //
        //             if (worldObj.func_147478_e(var12 + var4, var14, var13 + var5, true))
        //             {
        //                 worldObj.setBlock(var12 + var4, var14, var13 + var5, Blocks.snow_layer, 0, 2);
        //             }
        //         }
        //     }
        //
        //     BlockFalling.field_149832_M = false;
        // }
        //
        // /**
        //  * Two modes of operation: if passed true, save all Chunks in one go.  If passed false, save up to two chunks.
        //  * Return true if all chunks have been saved.
        //  */
        // public boolean saveChunks(boolean p_73151_1_, IProgressUpdate p_73151_2_)
        // {
        //     return true;
        // }
        //
        // /**
        //  * Save extra data not associated with any Chunk.  Not saved during autosave, only during world unload.  Currently
        //  * unimplemented.
        //  */
        // public void saveExtraData() {}
        //
        // /**
        //  * Unloads chunks that are marked to be unloaded. This is not guaranteed to unload every such chunk.
        //  */
        // public boolean unloadQueuedChunks()
        // {
        //     return false;
        // }
        //
        // /**
        //  * Returns if the IChunkProvider supports saving.
        //  */
        // public boolean canSave()
        // {
        //     return true;
        // }
        //
        // /**
        //  * Converts the instance data to a readable string.
        //  */
        // public String makeString()
        // {
        //     return "RandomLevelSource";
        // }
        //
        // /**
        //  * Returns a list of creatures of the specified type that can spawn at the given location.
        //  */
        // public List getPossibleCreatures(EnumCreatureType p_73155_1_, int p_73155_2_, int p_73155_3_, int p_73155_4_)
        // {
        //     BiomeGenBase var5 = worldObj.getBiomeGenForCoords(p_73155_2_, p_73155_4_);
        //     return p_73155_1_ == EnumCreatureType.monster && scatteredFeatureGenerator.func_143030_a(p_73155_2_, p_73155_3_, p_73155_4_) ? scatteredFeatureGenerator.getScatteredFeatureSpawnList() : var5.getSpawnableList(p_73155_1_);
        // }
        //
        // public ChunkPosition func_147416_a(World p_147416_1_, String p_147416_2_, int p_147416_3_, int p_147416_4_, int p_147416_5_)
        // {
        //     return "Stronghold".equals(p_147416_2_) && strongholdGenerator != null ? strongholdGenerator.func_151545_a(p_147416_1_, p_147416_3_, p_147416_4_, p_147416_5_) : null;
        // }
        //
        // public int getLoadedChunkCount()
        // {
        //     return 0;
        // }
        //
        // public void recreateStructures(int p_82695_1_, int p_82695_2_)
        // {
        //     if (mapFeaturesEnabled)
        //     {
        //         mineshaftGenerator.func_151539_a(this, worldObj, p_82695_1_, p_82695_2_, (Block[])null);
        //         villageGenerator.func_151539_a(this, worldObj, p_82695_1_, p_82695_2_, (Block[])null);
        //         strongholdGenerator.func_151539_a(this, worldObj, p_82695_1_, p_82695_2_, (Block[])null);
        //         scatteredFeatureGenerator.func_151539_a(this, worldObj, p_82695_1_, p_82695_2_, (Block[])null);
        //     }
        // }
    }
}