using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Serialization;

namespace M2_to_XML
{
    class Program
    {
        static int stamp;
        static Model model;
        static TransformationMatrix[] matrices;
        static int[] textureLookups;

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using(BinaryReader reader = new BinaryReader(File.Open(args[0], FileMode.Open)))
            {
                using(StreamWriter writer = new StreamWriter(args[0].Replace(".m2", ".xml")))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<Model>");
                    ReadChars(reader, 4); //MD20
                    Console.WriteLine();
                    ReadInt(reader); //version
                    Console.WriteLine();
                    Console.WriteLine("Name");
                    ReadName(reader, writer);
                    ReadInt(reader); //flags
                    Console.WriteLine();
                    ReadArray(reader); //sequences
                    Console.WriteLine();
                    Console.WriteLine("Animations");
                    ReadAnimations(reader);
                    ReadArray(reader); //animationlookup
                    Console.WriteLine();
                    ReadArray(reader); //d
                    Console.WriteLine();
                    Console.WriteLine("Bones");
                    ReadBones(reader, writer);
                    ReadArray(reader); //skeleton
                    Console.WriteLine();
                    Console.WriteLine("Vertices");
                    ReadVertices(reader, writer);
                    Console.WriteLine("Views");
                    ReadViews(reader, writer);
                    Console.WriteLine("Colors");
                    ReadColors(reader, writer);
                    Console.WriteLine("Textures");
                    ReadTextures(reader, writer);
                    Console.WriteLine("Transparencies");
                    ReadTransparencies(reader, writer);
                    ReadArray(reader); //i
                    Console.WriteLine();
                    ReadArray(reader); //textureanimations
                    Console.WriteLine();
                    ReadArray(reader); //texturereplace
                    Console.WriteLine();
                    Console.WriteLine("Blending");
                    ReadBlending(reader, writer);
                    ReadArray(reader); //bonelookup
                    Console.WriteLine();
                    Console.WriteLine("TextureLookups");
                    ReadTextureLookups(reader);
                    ReadArray(reader); //textureunits
                    Console.WriteLine();
                    ReadArray(reader); //transparencylookup
                    Console.WriteLine();
                    ReadArray(reader); //textureanimationlookup
                    Console.WriteLine();
                    ReadFloats(reader, 14); //floats
                    Console.WriteLine();
                    ReadArray(reader); //boundingtriangles
                    Console.WriteLine();
                    ReadArray(reader); //boundingvertices
                    Console.WriteLine();
                    ReadArray(reader); //boundingnormals
                    Console.WriteLine();
                    Console.WriteLine("Attachments");
                    ReadAttachments(reader, writer);
                    ReadArray(reader); //attachmentlookup
                    Console.WriteLine();
                    ReadArray(reader); //attachments2
                    Console.WriteLine();
                    ReadArray(reader); //lights
                    Console.WriteLine();
                    ReadArray(reader); //cameras
                    Console.WriteLine();
                    ReadArray(reader); //cameralookup
                    Console.WriteLine();
                    ReadArray(reader); //ribbons
                    Console.WriteLine();
                    ReadArray(reader); //particles
                    Console.WriteLine();
                    writer.WriteLine("</Model>");
                }
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Model));
            using(StreamReader reader = new StreamReader(args[0].Replace(".m2", ".xml")))
            {
                model = (Model)serializer.Deserialize(reader);
            }
            ReorderTextures();
            TransformModel();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            using(StreamWriter writer = new StreamWriter(args[0].Replace(".m2", ".xml")))
            {
                using(XmlWriter xml = XmlWriter.Create(writer, settings))
                {
                    serializer.Serialize(xml, model);
                }
            }
        }

        static void ReadName(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.Write("\t<Name>");
            for(int i = 1; i < size; i++)
            {
                writer.Write(reader.ReadChar());
            }
            writer.WriteLine("</Name>");
            reader.BaseStream.Position = position;
        }

        static void ReadAnimations(BinaryReader reader)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            short id;
            for(int i = 0; i < size; i++)
            {
                id = reader.ReadInt16();
                reader.ReadInt16();
                stamp = reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadSingle();
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadInt16();
                reader.ReadInt16();
                if(id == 0)
                {
                    break;
                }
            }
            reader.BaseStream.Position = position;
        }

        static void ReadBones(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Bones>");
            for(int i = 0; i < size; i++)
            {
                reader.ReadInt32();
                writer.WriteLine("\t\t<Bone>");
                writer.WriteLine("\t\t\t<Billboard>" + reader.ReadInt32() + "</Billboard>");
                writer.WriteLine("\t\t\t<Parent>" + reader.ReadInt16() + "</Parent>");
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                int sizeB = reader.ReadInt32();
                int offsetB = reader.ReadInt32();
                long positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                int index = -1;
                int value;
                for(int j = 0; j < sizeB; j++)
                {
                    value = reader.ReadInt32();
                    if(value == stamp)
                    {
                        index = j;
                    }
                }
                reader.BaseStream.Position = positionB;
                sizeB = reader.ReadInt32();
                offsetB = reader.ReadInt32();
                positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                if(sizeB > 0)
                {
                    if(index >= 0)
                    {
                        for(int j = 0; j < index; j++)
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        writer.WriteLine("\t\t\t<Translation x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\"/>");
                    }
                    else
                    {
                        writer.WriteLine("\t\t\t<Translation x=\"0\" y=\"0\" z=\"0\"/>");
                    }
                }
                else
                {
                    writer.WriteLine("\t\t\t<Translation x=\"0\" y=\"0\" z=\"0\"/>");
                }
                reader.BaseStream.Position = positionB;
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                sizeB = reader.ReadInt32();
                offsetB = reader.ReadInt32();
                positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                index = -1;
                for(int j = 0; j < sizeB; j++)
                {
                    value = reader.ReadInt32();
                    if(value == stamp)
                    {
                        index = j;
                    }
                }
                reader.BaseStream.Position = positionB;
                sizeB = reader.ReadInt32();
                offsetB = reader.ReadInt32();
                positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                if(sizeB > 0)
                {
                    if(index >= 0)
                    {
                        for(int j = 0; j < index; j++)
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        writer.WriteLine("\t\t\t<Rotation x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\" w=\"" + reader.ReadSingle() + "\"/>");
                    }
                    else
                    {
                        writer.WriteLine("\t\t\t<Rotation x=\"0\" y=\"0\" z=\"0\" w=\"1\"/>");
                    }
                }
                else
                {
                    writer.WriteLine("\t\t\t<Rotation x=\"0\" y=\"0\" z=\"0\" w=\"1\"/>");
                }
                reader.BaseStream.Position = positionB;
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                sizeB = reader.ReadInt32();
                offsetB = reader.ReadInt32();
                positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                index = -1;
                for(int j = 0; j < sizeB; j++)
                {
                    value = reader.ReadInt32();
                    if(value == stamp)
                    {
                        index = j;
                    }
                }
                reader.BaseStream.Position = positionB;
                sizeB = reader.ReadInt32();
                offsetB = reader.ReadInt32();
                positionB = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetB;
                if(sizeB > 0)
                {
                    writer.WriteLine("\t\t\t<Scale x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\"/>");
                }
                else
                {
                    writer.WriteLine("\t\t\t<Scale x=\"1\" y=\"1\" z=\"1\"/>");
                }
                reader.BaseStream.Position = positionB;
                writer.WriteLine("\t\t\t<Position x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\"/>");
                writer.WriteLine("\t\t</Bone>");
            }
            writer.WriteLine("\t</Bones>");
            reader.BaseStream.Position = position;
        }

        static void ReadVertices(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Vertices>");
            byte[] bones = new byte[4];
            byte[] weights = new byte[4];
            for(int i = 0; i < size; i++)
            {
                writer.WriteLine("\t\t<Vertex>");
                writer.WriteLine("\t\t\t<Position x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\"/>");
                for(int j = 0; j < 4; j++)
                {
                    weights[j] = reader.ReadByte();
                }
                for(int j = 0; j < 4; j++)
                {
                    bones[j] = reader.ReadByte();
                }
                writer.WriteLine("\t\t\t<Bones>");
                for(int j = 0; j < 4; j++)
                {
                    writer.WriteLine("\t\t\t\t<Bone index=\"" + bones[j] + "\" weight=\"" + weights[j] + "\"/>");
                }
                writer.WriteLine("\t\t\t</Bones>");
                writer.WriteLine("\t\t\t<Normal x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\" z=\"" + reader.ReadSingle() + "\"/>");
                writer.WriteLine("\t\t\t<Texture x=\"" + reader.ReadSingle() + "\" y=\"" + reader.ReadSingle() + "\"/>");
                writer.WriteLine("\t\t</Vertex>");
                reader.ReadSingle();
                reader.ReadSingle();
            }
            writer.WriteLine("\t</Vertices>");
            reader.BaseStream.Position = position;
        }

        static void ReadViews(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<View>");
            writer.WriteLine("\t\t<Indices>");
            int sizeV = reader.ReadInt32();
            int offsetV = reader.ReadInt32();
            long positionV = reader.BaseStream.Position;
            reader.BaseStream.Position = offsetV;
            for(int i = 0; i < sizeV; i++)
            {
                writer.WriteLine("\t\t\t<Index>" + reader.ReadInt16() + "</Index>");
            }
            reader.BaseStream.Position = positionV;
            writer.WriteLine("\t\t</Indices>");
            writer.WriteLine("\t\t<Triangles>");
            sizeV = reader.ReadInt32();
            offsetV = reader.ReadInt32();
            positionV = reader.BaseStream.Position;
            reader.BaseStream.Position = offsetV;
            for(int i = 0; i < sizeV; i++)
            {
                writer.WriteLine("\t\t\t<Triangle>" + reader.ReadInt16() + "</Triangle>");
            }
            reader.BaseStream.Position = positionV;
            writer.WriteLine("\t\t</Triangles>");
            reader.ReadInt32();
            reader.ReadInt32();
            writer.WriteLine("\t\t<Geosets>");
            sizeV = reader.ReadInt32();
            offsetV = reader.ReadInt32();
            positionV = reader.BaseStream.Position;
            reader.BaseStream.Position = offsetV;
            for(int i = 0; i < sizeV; i++)
            {
                reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                writer.WriteLine("\t\t\t<Geoset triangle=\"" + reader.ReadInt16() + "\" triangles=\"" + reader.ReadInt16() + "\"/>");
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
            }
            reader.BaseStream.Position = positionV;
            writer.WriteLine("\t\t</Geosets>");
            writer.WriteLine("\t\t<Textures>");
            sizeV = reader.ReadInt32();
            offsetV = reader.ReadInt32();
            positionV = reader.BaseStream.Position;
            reader.BaseStream.Position = offsetV;
            for(int i = 0; i < sizeV; i++)
            {
                reader.ReadInt16();
                reader.ReadInt16();
                writer.Write("\t\t\t<Texture geoset=\"" + reader.ReadInt16());
                reader.ReadInt16();
                writer.Write("\" color=\"" + reader.ReadInt16());
                writer.Write("\" blend=\"" + reader.ReadInt16());
                writer.Write("\" layer=\"" + reader.ReadInt16());
                reader.ReadInt16();
                writer.Write("\" texture=\"" + reader.ReadInt16());
                reader.ReadInt16();
                writer.WriteLine("\" transparency=\"" + reader.ReadInt16() + "\"/>");
                reader.ReadInt16();
            }
            reader.BaseStream.Position = positionV;
            writer.WriteLine("\t\t</Textures>");
            writer.WriteLine("\t</View>");
            reader.BaseStream.Position = position;
        }

        static void ReadColors(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Colors>");
            for(int i = 0; i < size; i++)
            {
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                int sizeC = reader.ReadInt32();
                int offsetC = reader.ReadInt32();
                long positionC = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetC;
                writer.Write("\t\t<Color red=\"" + reader.ReadSingle() + "\" green=\"" + reader.ReadSingle() + "\" blue=\"" + reader.ReadSingle());
                reader.BaseStream.Position = positionC;
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                sizeC = reader.ReadInt32();
                offsetC = reader.ReadInt32();
                positionC = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetC;
                short a = reader.ReadInt16();
                float alpha = a / 32767f;
                writer.WriteLine("\" alpha=\"" + alpha + "\"/>");
                reader.BaseStream.Position = positionC;
            }
            writer.WriteLine("\t</Colors>");
            reader.BaseStream.Position = position;
        }

        static void ReadTextures(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Textures>");
            for(int i = 0; i < size; i++)
            {
                writer.Write("\t\t<Texture type=\"" + reader.ReadInt32());
                reader.ReadInt32();
                writer.Write("\" file=\"");
                int sizeT = reader.ReadInt32();
                int offsetT = reader.ReadInt32();
                long positionT = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetT;
                for(int j = 1; j < sizeT; j++)
                {
                    writer.Write(reader.ReadChar());
                }
                reader.BaseStream.Position = positionT;
                writer.WriteLine("\"/>");
            }
            writer.WriteLine("\t</Textures>");
            reader.BaseStream.Position = position;
        }

        static void ReadTransparencies(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Transparencies>");
            for(int i = 0; i < size; i++)
            {
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                int sizeT = reader.ReadInt32();
                int offsetT = reader.ReadInt32();
                long positionT = reader.BaseStream.Position;
                reader.BaseStream.Position = offsetT;
                short a = reader.ReadInt16();
                float alpha = a / 32767f;
                writer.WriteLine("\t\t<Transparency>" + alpha + "</Transparency>");
                reader.BaseStream.Position = positionT;
            }
            writer.WriteLine("\t</Transparencies>");
            reader.BaseStream.Position = position;
        }

        static void ReadBlending(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Blending>");
            for(int i = 0; i < size; i++)
            {
                reader.ReadInt16();
                writer.WriteLine("\t\t<Blend>" + reader.ReadInt16() + "</Blend>");
            }
            writer.WriteLine("\t</Blending>");
            reader.BaseStream.Position = position;
        }

        static void ReadAttachments(BinaryReader reader, StreamWriter writer)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            writer.WriteLine("\t<Attachments>");
            for(int i = 0; i < size; i++)
            {
                writer.WriteLine("\t\t<Attachment id=\"" + reader.ReadInt32() + "\" bone=\"" + reader.ReadInt16() + "\"/>");
                reader.ReadInt16();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadSingle();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
            }
            writer.WriteLine("\t</Attachments>");
            reader.BaseStream.Position = position;
        }

        static void ReadTextureLookups(BinaryReader reader)
        {
            int size = reader.ReadInt32();
            int offset = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            textureLookups = new int[size];
            for(int i = 0; i < size; i++)
            {
                textureLookups[i] = reader.ReadInt16();
            }
            reader.BaseStream.Position = position;
        }

        static void ReorderTextures()
        {
            List<ModelTexture> textures = new List<ModelTexture>(model.Textures);
            int i;
            for(i = 0; i < textureLookups.Length; i++)
            {
                if(i == model.Textures.Length)
                {
                    break;
                }
                model.Textures[i] = textures[textureLookups[i]];
            }
            for(i = 0; i < model.Textures.Length; i++)
            {
                textures.Remove(model.Textures[i]);
            }
            for(; i < model.Textures.Length; i++)
            {
                model.Textures[i] = textures[0];
                textures.RemoveAt(0);
            }
        }

        static void TransformModel()
        {
            MakeBoneTranformation();
            ApplyParentTransformation();
            TransformBones();
            ApplyBoneTransformation();
            switch(model.Name)
            {
                case "HumanMale":
                    EyelidTransform(330, 342);
                    break;
                case "OrcMale":
                    EyelidTransform(24, 42);
                    break;
                case "OrcFemale":
                    EyelidTransform(204, 216);
                    break;
                case "DwarfMale":
                    EyelidTransform(18, 42);
                    break;
                case "DwarfFemale":
                    EyelidTransform(30, 42);
                    break;
                case "NightElfMale":
                    EyelidTransform(195, 207);
                    EyeGlowTransform(51, 0.05f, -0.01f);
                    break;
                case "NightElfFemale":
                    EyelidTransform(174, 186);
                    EyeGlowTransform(40, -0.005f, 0.005f);
                    break;
                case "TaurenMale":
                    EyelidTransform(36, 48);
                    break;
                case "GnomeMale":
                    EyelidTransform(6, 24);
                    break;
                case "GnomeFemale":
                    EyelidTransform(174, 192);
                    break;
                case "TrollMale":
                    EyelidTransform(1668, 1680);
                    break;
                case "TrollFemale":
                    FixBodyBack(0, 34, 6);
                    break;
            }
        }

        static void MakeBoneTranformation()
        {
            TransformationMatrix matrix;
            matrices = new TransformationMatrix[model.Bones.Length];
            for(int i = 0; i < model.Bones.Length; i++)
            {
                matrices[i] = new TransformationMatrix();
                matrices[i].Translate(new Vector3D(model.Bones[i].Position.x, model.Bones[i].Position.y, model.Bones[i].Position.z));
                matrices[i] *= TransformationMatrix.Translation(new Vector3D(model.Bones[i].Translation.x, model.Bones[i].Translation.y, model.Bones[i].Translation.z));
                matrices[i] *= TransformationMatrix.QuaterionRotation(new Quaternion(model.Bones[i].Rotation.x, model.Bones[i].Rotation.y, model.Bones[i].Rotation.z, model.Bones[i].Rotation.w));
                matrix = TransformationMatrix.Translation(new Vector3D(model.Bones[i].Position.x, model.Bones[i].Position.y, model.Bones[i].Position.z));
                matrix.Inverse();
                matrices[i] *= matrix;
            }
        }

        static void ApplyParentTransformation()
        {
            for(int i = 0; i < model.Bones.Length; i++)
            {
                if(model.Bones[i].Parent >= 0)
                {
                    matrices[i] = matrices[model.Bones[i].Parent] * matrices[i];
                }
            }
        }

        static void TransformBones()
        {
            PositionVector bone;
            for(int i = 0; i < model.Bones.Length; i++)
            {
                bone = new PositionVector(new Vector3D(model.Bones[i].Position.x, model.Bones[i].Position.y, model.Bones[i].Position.z));
                bone = matrices[i] * bone;
                model.Bones[i].Position = new ModelBonePosition
                {
                    x = bone[0, 0],
                    y = bone[2, 0],
                    z = -bone[1, 0]
                };
                Quaternion quaternion = matrices[i].ToQuaterion();
                model.Bones[i].Rotation = new ModelBoneRotation
                {
                    x = (float)quaternion.X,
                    y = (float)quaternion.Z,
                    z = -(float)quaternion.Y,
                    w = (float)quaternion.W
                };
            }
        }

        static void ApplyBoneTransformation()
        {
            PositionVector vertex;
            PositionVector position;
            PositionVector transformation;
            float temp;
            foreach(ModelVertex modelVertex in model.Vertices)
            {
                vertex = new PositionVector();
                vertex.Zero();
                position = new PositionVector(new Vector3D(modelVertex.Position.x, modelVertex.Position.y, modelVertex.Position.z));
                foreach(ModelVertexBone bone in modelVertex.Bones)
                {
                    if(bone.weight > 0)
                    {
                        transformation = matrices[bone.index] * position;
                        transformation *= ((float)bone.weight / 255f);
                        vertex += transformation;
                    }
                }
                modelVertex.Position = new ModelVertexPosition
                {
                    x = vertex[0, 0],
                    y = vertex[1, 0],
                    z = vertex[2, 0]
                };
                temp = -modelVertex.Position.y;
                modelVertex.Position.y = modelVertex.Position.z;
                modelVertex.Position.z = temp;
            }
        }

        static void EyelidTransform(int start, int end)
        {
            List<int> vertices = new List<int>();
            int index;
            for(int i = start; i < end; i++)
            {
                index = model.View.Indices[model.View.Triangles[i]];
                if(!vertices.Contains(index))
                {
                    vertices.Add(index);
                }
            }
            foreach(int vertex in vertices)
            {
                model.Vertices[vertex].Position.x -= 0.05f;
            }
        }

        static void EyeGlowTransform(int geoset, float x, float y)
        {
            List<int> vertices = new List<int>();
            List<int> bones = new List<int>();
            int index;
            for(int i = model.View.Geosets[geoset].triangle; i < model.View.Geosets[geoset].triangle + model.View.Geosets[geoset].triangles; i++)
            {
                index = model.View.Indices[model.View.Triangles[i]];
                if(!vertices.Contains(index))
                {
                    vertices.Add(index);
                }
            }
            foreach(int vertex in vertices)
            {
                model.Vertices[vertex].Position.x += x;
                model.Vertices[vertex].Position.y += y;
                index = model.Vertices[vertex].Bones[0].index;
                if(!bones.Contains(index))
                {
                    bones.Add(index);
                }
            }
            foreach(int bone in bones)
            {
                model.Bones[bone].Position.x += x;
                model.Bones[bone].Position.y += y;
            }
        }

        static void FixBodyBack(int body, int back, int count)
        {
            int[] triangles = new int[count];
            for(int i = 0; i < count; i++)
            {
                triangles[i] = model.View.Triangles[model.View.Geosets[back].triangle + i];
            }
            for(int i = model.View.Geosets[back].triangle - 1; i >= model.View.Geosets[body].triangle + model.View.Geosets[body].triangles; i--)
            {
                model.View.Triangles[i + count] = model.View.Triangles[i];
            }
            for(int i = 0; i < count; i++)
            {
                model.View.Triangles[model.View.Geosets[body].triangle + model.View.Geosets[body].triangles + i] = triangles[i];
            }
            model.View.Geosets[body].triangles += count;
            model.View.Geosets[back].triangle += count;
            model.View.Geosets[back].triangles -= count;
            for(int i = 0; i < back; i++)
            {
                model.View.Geosets[i].triangle += count;
            }
        }

        static void ReadChars(BinaryReader reader, int count)
        {
            for(int i = 0; i < count; i++)
            {
                ReadChar(reader);
            }
        }

        static void ReadChar(BinaryReader reader)
        {
            char temp = reader.ReadChar();
            Console.Write(temp);
        }

        static void ReadInt(BinaryReader reader)
        {
            int temp = reader.ReadInt32();
            Console.Write(temp);
        }

        static void ReadArray(BinaryReader reader)
        {
            ReadInt(reader);
            Console.Write(" ");
            ReadInt(reader);
        }

        static void ReadFloats(BinaryReader reader, int count)
        {
            for(int i = 0; i < count; i++)
            {
                ReadFloat(reader);
                Console.Write(" ");
            }
        }

        static void ReadFloat(BinaryReader reader)
        {
            float temp = reader.ReadSingle();
            Console.Write(temp);
        }
    }
}
