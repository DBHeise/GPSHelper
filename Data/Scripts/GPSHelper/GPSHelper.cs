

namespace GPSHelper
{
    using System;
    using System.Text;
    using Sandbox.ModAPI;
    using VRage.Game.Components;
    using VRage.Game.ModAPI;


    using Sandbox.Common.ObjectBuilders;
    using Sandbox.Definitions;
    using Sandbox.Game;
    using Sandbox.Game.Entities;
    using Sandbox.Game.EntityComponents;
    using Sandbox.ModAPI.Interfaces.Terminal;
    using VRage;
    using VRage.ModAPI;
    using VRage.ObjectBuilders;
    using VRage.Utils;
    using VRageMath;
    using System.Collections.Generic;
    using VRage.Voxels;
    using VRage.Game;

    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class GPSHelper : MySessionComponentBase
    {
        private Boolean isInitialized = false;

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();
            if (!this.isInitialized)
            {
                MyAPIGateway.Utilities.MessageEntered += Utilities_MessageEntered;
                this.isInitialized = true;
            }
        }

        protected override void UnloadData()
        {
            base.UnloadData();
            MyAPIGateway.Utilities.MessageEntered -= Utilities_MessageEntered;
            this.isInitialized = false;
        }


        private void Utilities_MessageEntered(string messageText, ref bool sendToOthers)
        {
            String cmd, extra;
            sendToOthers = false;

            int idx = messageText.IndexOf(' ');

            if (idx > 1)
            {
                cmd = messageText.Substring(0, idx).Trim();
                extra = messageText.Substring(idx).Trim();
            }
            else
            {
                cmd = messageText;
                extra = null;
            }
            switch (cmd.ToLowerInvariant())
            {
                case "/gpsx":
                    CreateGPS(extra);
                    break;
                case "/gpsx_reset":
                    ResetGPSCounter();
                    break;
                case "/gps_export":
                    ExportAllGPSToClipboard(extra);
                    break;
                case "/gps_toggle":
                    GPSToggle(extra);
                    break;
                case "/gps_off":
                    GPSToggle(extra, false);
                    break;
                case "/gps_on":
                    GPSToggle(extra, true);
                    break;
                case "/gps_remove":
                    RemoveGPS(extra);
                    break;
                default:
                    sendToOthers = true;
                    break;
            }
        }

        private const String baseStr = "0123456789ABCDEFGHIKJLMNOPQRSTUVWXYZ";
        public static String ConvertToBaseAlpha(double number)
        {

            String ans = "";
            int power = baseStr.Length;

            while ((int)number != 0)
            {
                var m = (int)(number % power);
                number /= power;
                ans = baseStr.Substring(m, 1) + ans;
            }
            return ans;
        }

        private void ResetGPSCounter()
        {
            var player = MyAPIGateway.Session.Player;
            var playerId = player.IdentityId;
            ulong autoid = 0;
            MyAPIGateway.Utilities.SetVariable("autoid_" + playerId, autoid);

        }

        private const double radius = 50.0;
        private void CreateGPS(String extra)
        {

            var player = MyAPIGateway.Session.Player;
            var playerPosition = player.Character.GetPosition();
            var playerId = player.IdentityId;

            ulong autoid = 0;
            MyAPIGateway.Utilities.GetVariable<ulong>("autoid_" + playerId, out autoid);
            autoid++;

            var voxelMaps = new List<MyVoxelBase>();
            var sphere = new BoundingSphereD(playerPosition, radius);

            MyGamePruningStructure.GetAllVoxelMapsInSphere(ref sphere, voxelMaps);

            StringBuilder builder = new StringBuilder();
            foreach (var map in voxelMaps)
            {
                if (map is MyPlanet)
                {
                    builder.AppendLine("Planet: " + map.AsteroidName.Split('-')[0]);
                } 
                else
                {
                    //var boundingBox = sphere.GetBoundingBox();
                    //Vector3I min = new Vector3I(boundingBox.Min);
                    //Vector3I max = new Vector3I(boundingBox.Max);
                    ////var min = default(Vector3I);
                    ////var max = default(Vector3I);
                    ////map.GetFilledStorageBounds(out min, out max);

                    //var tempStorage = new MyStorageData();
                    //tempStorage.Resize(min, max);
                    //map.Storage.ReadRange(tempStorage, MyStorageDataTypeFlags.Material, 0, min, max);

                    //var c = default(Vector3I);
                    //for (c.Z = min.Z; c.Z < max.Z; c.Z++)
                    //{
                    //    for (c.Y = min.Y; c.Y < max.Y; c.Y++)
                    //    {
                    //        for (c.X = min.X; c.X < max.X; c.X++)
                    //        {
                    //            var l = tempStorage.ComputeLinear(ref c);
                    //            var m = tempStorage.Material(l);
                    //            var material = MyDefinitionManager.Static.GetVoxelMaterialDefinition(m);
                    //            if (material.IsRare)
                    //            {
                    //                builder.Append("ORE: " + material.MaterialTypeName);
                    //            }
                    //        }
                    //    }
                    //}

                    //builder.AppendLine("================================");
                    //builder.AppendLine("DisplayNameText: " + map.DisplayNameText);
                    //builder.AppendLine("AstroidName: " + map.AsteroidName);
                    //builder.AppendLine("DebugName: " + map.DebugName);
                    //builder.AppendLine("DisplayName: " + map.DisplayName);
                    //builder.AppendLine("Name: " + map.Name);
                    //builder.AppendLine("MapTypeName: " + map.GetType().Name);
                    //builder.AppendLine("MapTopMostParentTypeName: " + map.GetTopMostParent().GetType().Name);
                    //builder.AppendLine();
                }
            }

            String locationName = String.IsNullOrWhiteSpace(extra) ? player.DisplayName : extra;

            locationName += " " + ConvertToBaseAlpha(autoid).PadLeft(3, '0');

            var gps = MyAPIGateway.Session.GPS.Create(locationName, builder.ToString(), playerPosition, true);
            MyAPIGateway.Session.GPS.AddGps(playerId, gps);

            MyAPIGateway.Utilities.SetVariable("autoid_" + playerId, autoid);

        }

        private void processGPS(String args, Action<IMyGps> gpsAction)
        {
            var gpsList = MyAPIGateway.Session.GPS.GetGpsList(MyAPIGateway.Session.Player.IdentityId);
            String matcher = String.IsNullOrWhiteSpace(args) ? null : args;
            foreach (var gps in gpsList)
            {
                if (matcher != null)
                {
                    if (gps.Name.Contains(matcher) || gps.Description.Contains(matcher) || gps.GPSColor.ToString().Contains(matcher))
                    {
                        gpsAction(gps);
                    }
                }
                else
                {
                    gpsAction(gps);
                }
            }            
        }

        private void ExportAllGPSToClipboard(String args)
        {
            StringBuilder sb = new StringBuilder();
            processGPS(args, gps =>
            {
                sb.AppendLine(gps.ToString());
            });
            VRage.Utils.MyClipboardHelper.SetClipboard(sb.ToString());
            MyAPIGateway.Utilities.ShowNotification("GPS Points have been exported to the clipboard");
        }

        private void GPSToggle(String args, bool? setState = null)
        {
            processGPS(args, gps =>
            {
                gps.ShowOnHud = setState.HasValue ? setState.Value : !(gps.ShowOnHud);
                MyAPIGateway.Session.GPS.ModifyGps(MyAPIGateway.Session.Player.IdentityId, gps);
            });
            MyAPIGateway.Utilities.ShowNotification("GPS Points have been modified");
        }

        private void RemoveGPS(String args)
        {
            processGPS(args, gps =>
            {
                MyAPIGateway.Session.GPS.RemoveGps(MyAPIGateway.Session.Player.IdentityId, gps);
            });
            MyAPIGateway.Utilities.ShowNotification("GPS Points have been removed");
        }
    }
}
