using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class Interação : MonoBehaviour
{
    [Header("Objeto interagível")]
    [SerializeField] private GameObject ObjetoInteragivel;
    [SerializeField] private Camera CameraDoJogador;
    [SerializeField] private float DistanciaMaxima = 5f;

    [Header("Vídeo")]
    [SerializeField] private GameObject ObjetoVideo;
    [SerializeField] private VideoPlayer VideoSkybox;

    [Header("Partículas")]
    [SerializeField] private Sistemaparticulas SistemaFumaca;

    [Header("Outline")]
    [SerializeField] private Color CorOutline = Color.white;
    [SerializeField] private float EspessuraOutline = 0.025f;

    private LineRenderer outline;
    private bool objetoInteragivelDetectado;
    private bool interacaoConcluida;

    private void Start()
    {
        if (CameraDoJogador == null)
        {
            CameraDoJogador = GetComponentInChildren<Camera>();
        }

        if (CameraDoJogador == null)
        {
            CameraDoJogador = Camera.main;
        }

        CriarOutline();

        if (ObjetoVideo != null)
        {
            ObjetoVideo.SetActive(false);
        }
    }

    private void Update()
    {
        VerificarObjetoInteragivel();

        if (objetoInteragivelDetectado && InteracaoFoiPressionada())
        {
            Interagir();
        }
    }

    private void VerificarObjetoInteragivel()
    {
        if (interacaoConcluida)
        {
            outline.enabled = false;
            objetoInteragivelDetectado = false;
            return;
        }

        if (ObjetoInteragivel == null || CameraDoJogador == null)
        {
            outline.enabled = false;
            objetoInteragivelDetectado = false;
            return;
        }

        Ray raio = new Ray(
            CameraDoJogador.transform.position,
            CameraDoJogador.transform.forward);

        bool encontrouObjeto = Physics.Raycast(
            raio,
            out RaycastHit informacaoColisao,
            DistanciaMaxima) &&
            (informacaoColisao.transform == ObjetoInteragivel.transform ||
             informacaoColisao.transform.IsChildOf(ObjetoInteragivel.transform));

        objetoInteragivelDetectado = encontrouObjeto;
        outline.enabled = encontrouObjeto;

        if (encontrouObjeto)
        {
            AtualizarOutline();
        }
    }

    private bool InteracaoFoiPressionada()
    {
        bool tecladoFoiPressionado =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        bool gamepadFoiPressionado =
            Gamepad.current != null &&
            Gamepad.current.buttonWest.wasPressedThisFrame;

        return tecladoFoiPressionado || gamepadFoiPressionado;
    }

    private void Interagir()
    {
        interacaoConcluida = true;
        objetoInteragivelDetectado = false;
        outline.enabled = false;

        if (ObjetoVideo != null)
        {
            ObjetoVideo.SetActive(true);

            VideoPlayer videoPlayer =
                ObjetoVideo.GetComponent<VideoPlayer>();

            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
            else
            {
                Debug.LogWarning(
                    "O ObjetoVideo não possui um componente VideoPlayer.");
            }
        }

        if (SistemaFumaca != null)
        {
            SistemaFumaca.IniciarContagem();
        }
    }

    private void CriarOutline()
    {
        GameObject objetoOutline = new GameObject("Outline Branco");
        objetoOutline.transform.SetParent(transform);

        outline = objetoOutline.AddComponent<LineRenderer>();
        outline.useWorldSpace = true;
        outline.loop = false;
        outline.positionCount = 24;
        outline.startWidth = EspessuraOutline;
        outline.endWidth = EspessuraOutline;
        outline.startColor = CorOutline;
        outline.endColor = CorOutline;
        outline.material = CriarMaterialDoOutline();
        outline.enabled = false;
    }

    private void AtualizarOutline()
    {
        Renderer[] renderizadores =
            ObjetoInteragivel.GetComponentsInChildren<Renderer>();

        if (renderizadores.Length == 0)
        {
            return;
        }

        Bounds limites = renderizadores[0].bounds;

        for (int indice = 1; indice < renderizadores.Length; indice++)
        {
            limites.Encapsulate(renderizadores[indice].bounds);
        }

        Vector3 centro = limites.center;
        Vector3 tamanho = limites.extents;

        Vector3 inferiorEsquerdoFrente =
            centro + new Vector3(-tamanho.x, -tamanho.y, -tamanho.z);

        Vector3 inferiorDireitoFrente =
            centro + new Vector3(tamanho.x, -tamanho.y, -tamanho.z);

        Vector3 inferiorDireitoTras =
            centro + new Vector3(tamanho.x, -tamanho.y, tamanho.z);

        Vector3 inferiorEsquerdoTras =
            centro + new Vector3(-tamanho.x, -tamanho.y, tamanho.z);

        Vector3 superiorEsquerdoFrente =
            centro + new Vector3(-tamanho.x, tamanho.y, -tamanho.z);

        Vector3 superiorDireitoFrente =
            centro + new Vector3(tamanho.x, tamanho.y, -tamanho.z);

        Vector3 superiorDireitoTras =
            centro + new Vector3(tamanho.x, tamanho.y, tamanho.z);

        Vector3 superiorEsquerdoTras =
            centro + new Vector3(-tamanho.x, tamanho.y, tamanho.z);

        Vector3[] pontos =
        {
            inferiorEsquerdoFrente,
            inferiorDireitoFrente,
            inferiorDireitoFrente,
            inferiorDireitoTras,
            inferiorDireitoTras,
            inferiorEsquerdoTras,
            inferiorEsquerdoTras,
            inferiorEsquerdoFrente,

            superiorEsquerdoFrente,
            superiorDireitoFrente,
            superiorDireitoFrente,
            superiorDireitoTras,
            superiorDireitoTras,
            superiorEsquerdoTras,
            superiorEsquerdoTras,
            superiorEsquerdoFrente,

            inferiorEsquerdoFrente,
            superiorEsquerdoFrente,
            inferiorDireitoFrente,
            superiorDireitoFrente,
            inferiorDireitoTras,
            superiorDireitoTras,
            inferiorEsquerdoTras,
            superiorEsquerdoTras
        };

        outline.SetPositions(pontos);
    }

    private Material CriarMaterialDoOutline()
    {
        Shader shader = Shader.Find("Sprites/Default");
        Material material = new Material(shader);
        material.color = CorOutline;
        return material;
    }
}
