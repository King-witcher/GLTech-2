using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    public unsafe class Scene2
    {
        internal unsafe SceneData* mapptr;
        internal unsafe Camera_* cameraptr;
        private Camera refCamera;
        private Scene refMap;

        public Scene2(Scene map, Camera camera)
        {
            mapptr = map.unmanaged;
            cameraptr = camera.unmanaged;
            refCamera = camera;
            refMap = map;
        }

        public Scene Geometry
        {
            get => refMap;
        }

        //public Material Background
        //{
            //get => refCamera.Background;
            //set => refCamera.Background = value;
        //}
        public Vector CameraPosition
        {
            get => cameraptr->camera_position;
            set => cameraptr->camera_position = value;
        }
        public float CameraRotation
        {
            get => cameraptr->camera_angle;
            set => cameraptr->camera_angle = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddWall(Wall w)
        {
            if (mapptr->wall_count >= mapptr->wall_max)
                throw new IndexOutOfRangeException("Wall limit reached.");
            mapptr->Add(w.unmanaged);
        }

        public void AddWalls(params Wall[] walls)
        {
            foreach (Wall wall in walls)
            {
                if (wall == null)
                    break;
                AddWall(wall);
            }
        }
    }
}