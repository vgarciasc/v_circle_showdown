#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif
 
namespace UnityEngine.Sprites
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class NineSlicedProvider : MonoBehaviour
    {
        [Header("Target")]
        public Sprite sprite;
        public Material material;
 
        [Header("Info")]
        public Color colour = Color.white;
        public Mode mode = Mode.NineSlice;
 
        private MeshFilter meshFilter { get { return GetComponent<MeshFilter>(); } }
        private MeshRenderer meshRenderer { get { return GetComponent<MeshRenderer>(); } }
 
        public enum BoundsMode
        {
            None = 0,
            Fit = 1
        }
 
        public enum Mode
        {
            Single = 0,
            NineSlice = 1
        }
 
        [Header("9-Slice")]
        public float sliceWidth = 1f;
        public float sliceHeight = 1f;
 
        [Header("Bounds")]
        public float boundsWidth = 1f;
        public float boundsHeight = 1f;
 
        [Header("Scaling")]
        public BoundsMode boundsMode = BoundsMode.None;
 
        [Header("Debugging")]
        public Rect textureRect;
        public Vector2 textureRectOffset;
        public Rect rect;
        public Texture2D texture;
 
        private Mesh mesh
        {
            get
            {
                return Application.isPlaying ? meshFilter.mesh : meshFilter.sharedMesh;
            }
            set
            {
                if (Application.isPlaying) meshFilter.mesh = value;
                if (!Application.isPlaying) meshFilter.sharedMesh = value;
            }
        }
 
#if UNITY_EDITOR
        [SerializeField]
        private int insCheckId;
#endif
 
        void Awake()
        {
            ApplyRendererProps();
            Reload();
        }
 
#if UNITY_EDITOR
        private void OnEnable()
        {
            meshFilter.hideFlags = HideFlags.HideInInspector;
            meshRenderer.hideFlags = HideFlags.HideInInspector;
        }
 
        private void OnDisable()
        {
            meshFilter.hideFlags = HideFlags.None;
            meshRenderer.hideFlags = HideFlags.None;
        }
 
        void OnDrawGizmosSelected()
        {
            //Duplicate/Copy paste fix.
            if (insCheckId != GetInstanceID())
            {
                Debug.Log("Instance id changed, reloading mesh");
                insCheckId = GetInstanceID();
                mesh = null;
                Reload();
            }
            if ( !Application.isPlaying && mode == Mode.NineSlice)
            {
                Reload();
            }
            if (boundsMode != BoundsMode.None)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(boundsWidth * transform.localScale.x, boundsHeight * transform.localScale.y, 0.1f * transform.localScale.z));
            }
        }
 
        /// <summary>
        /// This is used to apply the atlas texture to the sprite on play/build.
        /// </summary>
        [PostProcessScene(2)]
        public static void OnPostprocessScene()
        {
            Debug.Log("NineSlicedProvider::OnPostprocessScene()");
            var all = FindObjectsOfType<NineSlicedProvider>();
            foreach (var item in all)
            {
                item.Reload();
            }
        }
#endif
 
        public void ApplyRendererProps()
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            //Not needed? Seems pointless.
            GetComponent<Renderer>().GetPropertyBlock(props);
            //props.Clear();
            props.SetColor("_Color", colour);
            props.SetTexture("_MainTex", sprite.texture);
            GetComponent<Renderer>().SetPropertyBlock(props);
        }
 
        [ContextMenu("Reload")]
        public void Reload()
        {
            if (sprite == null)return;
 
            meshRenderer.sharedMaterial = material;
            //Create mesh
            Mesh newMesh = null;
            if (mode == Mode.Single)
            {
                //Load from sprite? TODO
                newMesh = null;
            }
            else
            {
                //Create.
                newMesh = CreateSliced();
            }
 
            if ( newMesh != null )
            {
                //Set alignment. Yes this should be part of the CreateSliced function, I just haven't bothered.
                newMesh = ScaleAndAlign(newMesh);
                //Generate bounds.
                newMesh.RecalculateBounds();
                //Set mesh
                newMesh.name = name;
                mesh = newMesh;
                //Update the colour.
                ApplyRendererProps();
            }
 
            if (Application.isPlaying)
            {
                Debug.Log("NineSlicedProvider::Reload() " + gameObject.name + " " + sprite.name + " " + sprite.texture.name);
            }
        }
 
        Rect GetBounds(Vector3[] verts)
        {
            Rect bounds = new Rect();
            for (int i = 0; i < verts.Length; i++)
            {
                //Get bounds.
                Vector3 vert = verts[i];
                bounds.yMin = Mathf.Min(bounds.yMin, vert.y);
                bounds.xMin = Mathf.Min(bounds.xMin, vert.x);
                bounds.yMax = Mathf.Max(bounds.yMax, vert.y);
                bounds.xMax = Mathf.Max(bounds.xMax, vert.x);
            }
            return bounds;
        }
 
        Mesh ScaleAndAlign(Mesh target)
        {
            var verts = target.vertices;
            Rect bounds = GetBounds(verts);
            //Set sizing.
            var worldHeight = bounds.height;
            var  worldWidth = bounds.width;
            //Do scaling..
            var finalScale = 1f;
            if (boundsMode == BoundsMode.Fit)
            {
                var scaleY = Mathf.Min(worldHeight, boundsHeight) / worldHeight;
                var scaleX = Mathf.Min(worldWidth, boundsWidth) / worldWidth;
                finalScale = Mathf.Min(scaleX, scaleY);
            }
            worldWidth *= finalScale;
            worldHeight *= finalScale;
            //Scale
            for (int i = 0; i < verts.Length; i++)
            {
                var v = verts[i];
                verts[i] = new Vector3(v.x  * finalScale, v.y * finalScale, v.z);
            }
            //Set.
            target.vertices = verts;
            return target;
        }
 
        Mesh CreateSliced()
        {
            textureRect = sprite.textureRect;
            texture = sprite.texture;
            rect = sprite.rect;
            textureRectOffset = sprite.textureRectOffset;
 
            var verts = new Vector3[4 * 9];
            var uv = new Vector2[4 * 9];
            var triangles = new int[6 * 9];
 
            var Width = sprite.rect.width * sliceWidth;
            var Height = sprite.rect.height * sliceHeight;
            var Left = sprite.border.x;
            var Right = Width - sprite.border.z;
            var Top = sprite.border.y;
            var Bottom = Height - sprite.border.w;
 
            var offset = -new Vector3( sprite.pivot.x *sliceWidth, sprite.pivot.y * sliceHeight );
 
            //Verts.
            var x1 = (offset.x + 0f) / sprite.pixelsPerUnit;
            var x2 = (offset.x + Left) / sprite.pixelsPerUnit;
            var x3 = (offset.x + Right) / sprite.pixelsPerUnit;
            var x4 = (offset.x + Width) / sprite.pixelsPerUnit;
            var y1 = (offset.y + 0f) / sprite.pixelsPerUnit;
            var y2 = (offset.y + Top) / sprite.pixelsPerUnit;
            var y3 = (offset.y + Bottom) / sprite.pixelsPerUnit;
            var y4 = (offset.y + Height) / sprite.pixelsPerUnit;
 
            //UVs.
            var u1 = sprite.textureRect.xMin / sprite.texture.width;
            var u2 = (sprite.textureRect.xMin + sprite.border.x ) / sprite.texture.width;
            var u3 = (sprite.textureRect.xMax - sprite.border.z ) / sprite.texture.width;
            var u4 = sprite.textureRect.xMax / sprite.texture.width;
            var v1 = sprite.textureRect.yMin / sprite.texture.height;
            var v2 = (sprite.textureRect.yMin + sprite.border.y) / sprite.texture.height;
            var v3 = (sprite.textureRect.yMax - sprite.border.w) / sprite.texture.height;
            var v4 = sprite.textureRect.yMax / sprite.texture.height;
 
            int index = -4;
 
            //Top row.
            Set(verts, uv, triangles, index += 4,
                new Vector3(x1, y1), new Vector3(x2, y1), new Vector3(x2, y2), new Vector3(x1, y2),
                new Vector2(u1, v1), new Vector2(u2, v1), new Vector2(u2, v2), new Vector2(u1, v2));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x2, y1), new Vector3(x3, y1), new Vector3(x3, y2), new Vector3(x2, y2),
                new Vector2(u2, v1), new Vector2(u3, v1), new Vector2(u3, v2), new Vector2(u2, v2));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x3, y1), new Vector3(x4, y1), new Vector3(x4, y2), new Vector3(x3, y2),
                new Vector2(u3, v1), new Vector2(u4, v1), new Vector2(u4, v2), new Vector2(u3, v2));
 
            //Middle row.
            Set(verts, uv, triangles, index += 4,
               new Vector3(x1, y2), new Vector3(x2, y2), new Vector3(x2, y3), new Vector3(x1, y3),
               new Vector2(u1, v2), new Vector2(u2, v2), new Vector2(u2, v3), new Vector2(u1, v3));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x2, y2), new Vector3(x3, y2), new Vector3(x3, y3), new Vector3(x2, y3),
                new Vector2(u2, v2), new Vector2(u3, v2), new Vector2(u3, v3), new Vector2(u2, v3));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x3, y2), new Vector3(x4, y2), new Vector3(x4, y3), new Vector3(x3, y3),
                new Vector2(u3, v2), new Vector2(u4, v2), new Vector2(u4, v3), new Vector2(u3, v3));
 
            //Bottom row.
            Set(verts, uv, triangles, index += 4,
               new Vector3(x1, y3), new Vector3(x2, y3), new Vector3(x2, y4), new Vector3(x1, y4),
               new Vector2(u1, v3), new Vector2(u2, v3), new Vector2(u2, v4), new Vector2(u1, v4));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x2, y3), new Vector3(x3, y3), new Vector3(x3, y4), new Vector3(x2, y4),
                new Vector2(u2, v3), new Vector2(u3, v3), new Vector2(u3, v4), new Vector2(u2, v4));
 
            Set(verts, uv, triangles, index += 4,
                new Vector3(x3, y3), new Vector3(x4, y3), new Vector3(x4, y4), new Vector3(x3, y4),
                new Vector2(u3, v3), new Vector2(u4, v3), new Vector2(u4, v4), new Vector2(u3, v4));
 
            //Use existing mesh if it fits.
            var d = mesh != null && mesh.vertices.Length == verts.Length ? mesh : new Mesh();
         
            //Set mesh propertys.
            d.vertices = verts;
            d.uv = uv;
            d.triangles = triangles;
       
            //Colors could be used, mostly pointless as we have tint.
            var c = new Color[verts.Length];
            for ( int i = 0; i < c.Length; i ++ )
            {
                c[i] = Color.white;
            }
            d.colors = c;
 
            //Done.
            return d;
        }
 
        void Set(Vector3[] verts, Vector2[] uvs, int[] triangles, int index, Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl, Vector2 utl, Vector2 utr, Vector2 ubr, Vector2 ubl)
        {
            verts[index] = tl;
            verts[index + 1] = bl;
            verts[index + 2] = br;
            verts[index + 3] = tr;
 
            uvs[index] = utl;
            uvs[index + 1] = ubl;
            uvs[index + 2] = ubr;
            uvs[index + 3] = utr;
 
            int triindex = index / 4 * 6;
 
            triangles[triindex] = index;
            triangles[triindex + 1] = index + 1;
            triangles[triindex + 2] = index + 2;
            triangles[triindex + 3] = index;
            triangles[triindex + 4] = index + 2;
            triangles[triindex + 5] = index + 3;
        }
    }
}