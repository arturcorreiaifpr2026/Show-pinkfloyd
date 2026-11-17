using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class Interação : MonoBehaviour
{
    [Header("Objeto interagível")]
    [SerializeField] private GameObject ObjetoInteragivel;
    [SerializeField] private Camera CameraDoJogador;
    [SerializeField] private float DistanciaMaxima = 5f;

    [Header("Vídeo da skybox")]
    [SerializeField] private VideoPlayer VideoSkybox;

    [Header("Cubo de seleção")]
    [SerializeField] private Color CorOutline = Color.white;
    [SerializeField] private float EspessuraOutline = 0.025f;

    [SerializeField] private UnityEvent AoInteragir;

    private LineRenderer outline;
    private bool objetoSelecionado;

    private void Start()
    {
        if (CameraDoJogador == null)
        {
            CameraDoJogador = GetComponentInChildren<Camera>();
        }

        CriarCuboOutline();
        outline.enabled = false;
    }

    private void Update()
    {
        VerificarObjetoSelecionado();

        if (objetoSelecionado && InteracaoFoiPressionada())
        {
            ReproduzirVideo();
            AoInteragir.Invoke();
        }
    }

    private void VerificarObjetoSelecionado()
    {
        if (ObjetoInteragivel == null || CameraDoJogador == null)
        {
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

        objetoSelecionado = encontrouObjeto;
        outline.enabled = encontrouObjeto;

        if (encontrouObjeto)
        {
            AtualizarCuboOutline();
        }
    }

    private bool InteracaoFoiPressionada()
    {
        bool tecladoFoiPressionado =
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame;

        bool controleFoiPressionado =
            Gamepad.current != null &&
            Gamepad.current.buttonWest.wasPressedThisFrame;

        bool pressionado = tecladoFoiPressionado || controleFoiPressionado;

        if (pressionado)
        {
            Debug.Log("Interação foi pressionada.");
        }

        return pressionado;
    }

    private void ReproduzirVideo()
    {
        if (VideoSkybox == null)
        {
            Debug.LogError("O campo VideoSkybox não foi configurado no Inspector.");
            return;
        }

        VideoSkybox.enabled = true;
        VideoSkybox.Play();

        AoInteragir?.Invoke();

        Debug.Log("Vídeo iniciado e contagem da partícula ativada.");
    }

    private void CriarCuboOutline()
    {
        GameObject objetoCubo = new GameObject("Cubo Outline Branco");
        objetoCubo.transform.SetParent(transform);

        outline = objetoCubo.AddComponent<LineRenderer>();
        outline.useWorldSpace = true;
        outline.loop = false;
        outline.positionCount = 24;
        outline.startWidth = EspessuraOutline;
        outline.endWidth = EspessuraOutline;
        outline.startColor = CorOutline;
        outline.endColor = CorOutline;
        outline.material = CriarMaterialDoOutline();
    }

    private void AtualizarCuboOutline()
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
