using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using TensorFlowLite;

[RequireComponent(typeof(Button))]
public class QuizManager : MonoBehaviour
{
    public TextAsset tfliteModelAsset;
    public string modelKey = "JWST";
    public Button quizButton;
    public UnityEvent onQuizStarted;

    private Interpreter interpreter;
    private int inputIndex = 0;
    private int outputIndex = 0;
    private int vocabSize = 4;
    private int outputCols = 0;
    private List<Question> bank;
    private List<Question> currentQuestions;

    [Serializable]
    private class Question
    {
        public string Q;
        public string[] Options;
        public int CorrectIndex;
        public Question(string q, string[] o, int c)
        {
            Q = q;
            Options = o;
            CorrectIndex = c;
        }
    }

    void Awake()
    {
        if (quizButton == null) quizButton = GetComponent<Button>();

        if (tfliteModelAsset != null)
        {
            interpreter = new Interpreter(tfliteModelAsset.bytes, new InterpreterOptions() { threads = 2 });
            interpreter.AllocateTensors();

            // Determine vocabSize from input shape
            var inInfo = interpreter.GetInputTensorInfo(inputIndex);
            var inShape = inInfo.shape;
            vocabSize = (inShape.Length > 1) ? inShape[1] : inShape[0];

            // Determine outputCols from output shape
            var outInfo = interpreter.GetOutputTensorInfo(outputIndex);
            var outShape = outInfo.shape;
            outputCols = (outShape.Length > 1) ? outShape[1] : outShape[0];
        }

        bank = GetBank(modelKey);
    }

    void OnEnable()
    {
        quizButton.onClick.AddListener(OnQuizButtonClicked);
    }

    void OnDisable()
    {
        quizButton.onClick.RemoveListener(OnQuizButtonClicked);
    }

    public void OnQuizButtonClicked()
    {
        onQuizStarted?.Invoke();
        bank = GetBank(modelKey);

        if (interpreter != null)
        {
            currentQuestions = PickWithTFLite(modelKey);
        }
        else
        {
            currentQuestions = bank.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
        }

        Debug.Log($"--- QUIZ START ({modelKey}) ---");
        for (int i = 0; i < currentQuestions.Count; i++)
        {
            var q = currentQuestions[i];
            Debug.Log($"Q{i + 1}: {q.Q}");
            for (int j = 0; j < q.Options.Length; j++)
            {
                Debug.Log($"   {(char)('A' + j)}. {q.Options[j]}");
            }
        }
        Debug.Log("--- QUIZ END ---");
    }

    public void OnOptionSelected(int questionIndex, int optionIndex)
    {
        if (currentQuestions == null || questionIndex < 0 || questionIndex >= currentQuestions.Count) return;
        var q = currentQuestions[questionIndex];
        bool correct = optionIndex == q.CorrectIndex;
        Debug.Log(correct ? "Correct!" : "Incorrect.");
        Debug.Log($"Answer: {(char)('A' + q.CorrectIndex)}");
    }

    private List<Question> PickWithTFLite(string key)
    {
        // 1) Build the [1, vocabSize] one‑hot input
        float[,] input = new float[1, vocabSize];
        var keys = new[] { "JWST", "PerseveranceRover", "TurbofanEngine", "Starlink" };
        int idx = Array.IndexOf(keys, key);
        if (idx >= 0) input[0, idx] = 1f;

        interpreter.SetInputTensorData(inputIndex, input);
        interpreter.Invoke();

        // 2) Read back [1, outputCols] scores
        float[,] rawScores = new float[1, outputCols];
        interpreter.GetOutputTensorData(outputIndex, rawScores);

        // 3) Flatten and sort
        var scored = new List<(float score, int idx)>();
        for (int i = 0; i < outputCols; i++)
        {
            scored.Add((rawScores[0, i], i));
        }
        var top = scored
            .OrderByDescending(x => x.score)
            .Take(10)
            .Select(x => x.idx < bank.Count ? bank[x.idx] : null)
            .Where(q => q != null)
            .ToList();

        // 4) If we got fewer than 10, fill up with random uniques
        if (top.Count < 10)
        {
            var fill = bank
                .Where(q => !top.Contains(q))
                .OrderBy(_ => Guid.NewGuid())
                .Take(10 - top.Count);
            top.AddRange(fill);
        }

        return top;
    }

    private List<Question> GetBank(string key)
    {
        switch (key)
        {
            case "JWST":
                return new List<Question>
                {
                    new Question("In which year was JWST launched?", new[]{"2018","2020","2021","2022"}, 2),
                    new Question("What does JWST stand for?", new[]{"James Web Science Telescope","James Webb Space Telescope","Jet Wave Spectrum Telescope","Juno Wide Space Telescope"}, 1),
                    new Question("Which telescope did JWST succeed?", new[]{"Chandra","Spitzer","Kepler","Hubble"}, 3),
                    new Question("What type of light does JWST mainly observe?", new[]{"Visible","Infrared","X-ray","Ultraviolet"}, 1),
                    new Question("Where is JWST located?", new[]{"Low Earth Orbit","Moon Orbit","Lagrange Point L2","Geostationary Orbit"}, 2),
                    new Question("Which agency built the JWST?", new[]{"NASA","ESA","JAXA","CNSA"}, 0),
                    new Question("What is the diameter of JWST's mirror?", new[]{"3.5 meters","4.5 meters","6.5 meters","10 meters"}, 2),
                    new Question("What is the purpose of the sunshield on JWST?", new[]{"Power generation","Radiation protection","Thermal control","Communication"}, 2),
                    new Question("Which rocket launched the JWST?", new[]{"Atlas V","Falcon 9","Ariane 5","Soyuz"}, 2),
                    new Question("JWST mainly studies?", new[]{"Black holes","Galaxy formation","Earth climate","Mars geology"}, 1),
                };
            case "PerseveranceRover":
                return new List<Question>
                {
                    new Question("On which planet did Perseverance land?", new[]{"Mars","Moon","Venus","Mercury"}, 0),
                    new Question("Which organization launched Perseverance?", new[]{"NASA","ESA","Roscosmos","ISRO"}, 0),
                    new Question("What year did Perseverance land?", new[]{"2018","2019","2020","2021"}, 3),
                    new Question("What helicopter accompanied Perseverance?", new[]{"Buzz","Flyer","Ingenuity","Hover"}, 2),
                    new Question("What is Perseverance's mission?", new[]{"Study the Sun","Explore Jupiter","Search for life on Mars","Map Earth"}, 2),
                    new Question("What is the name of the landing site?", new[]{"Valles Marineris","Olympus Mons","Jezero Crater","Gale Crater"}, 2),
                    new Question("Which rover is a twin of Perseverance?", new[]{"Curiosity","Opportunity","Spirit","None"}, 0),
                    new Question("How many wheels does Perseverance have?", new[]{"4","6","8","10"}, 1),
                    new Question("Perseverance is powered by?", new[]{"Solar panels","Battery","Nuclear power","Wind turbine"}, 2),
                    new Question("What feature allows Perseverance to collect samples?", new[]{"Drill","Laser","Microscope","Radar"}, 0),
                };
            case "TurbofanEngine":
                return new List<Question>
                {
                    new Question("A turbofan engine has a large fan at the front.", new[]{"True","False"}, 0),
                    new Question("Which part generates most thrust in modern turbofans?", new[]{"Combustion","Fan","Nozzle","Turbine"}, 1),
                    new Question("What is bypass ratio?", new[]{"Fuel to air ratio","Air bypassing core vs air through core","Engine size","Thrust ratio"}, 1),
                    new Question("Turbofans are used mostly in?", new[]{"Helicopters","Fighter jets","Commercial airliners","Drones"}, 2),
                    new Question("Turbofan engines are more efficient at?", new[]{"Subsonic speeds","Supersonic speeds","Stationary","Vertical takeoff"}, 0),
                    new Question("What does the turbine do?", new[]{"Burn fuel","Spin the fan","Cool the engine","Measure speed"}, 1),
                    new Question("Which part compresses air before combustion?", new[]{"Nozzle","Combustor","Compressor","Intake"}, 2),
                    new Question("After burning, gases exit through?", new[]{"Fan","Compressor","Exhaust nozzle","Turbine"}, 2),
                    new Question("Which engine has a higher bypass ratio?", new[]{"Military jet","Fighter engine","Airliner engine","Rocket"}, 2),
                    new Question("Turbofan efficiency improves with?", new[]{"Higher altitudes","Lower bypass","Narrow fans","Slower speeds"}, 0),
                };
            case "Starlink":
                return new List<Question>
                {
                    new Question("What is Starlink?", new[]{"Space telescope","Mars rover","Internet satellites","Lunar base"}, 2),
                    new Question("Who operates Starlink?", new[]{"NASA","ESA","Blue Origin","SpaceX"}, 3),
                    new Question("Starlink provides internet via?", new[]{"Cables","Towers","Satellites","Lasers"}, 2),
                    new Question("Where are Starlink satellites deployed?", new[]{"LEO","MEO","GEO","Deep Space"}, 0),
                    new Question("When did Starlink begin deployment?", new[]{"2015","2018","2019","2021"}, 2),
                    new Question("What is a major concern about Starlink?", new[]{"Slow speed","Too expensive","Space debris","It blocks GPS"}, 2),
                    new Question("Starlink aims to help with?", new[]{"Deep space exploration","Mars settlement","Rural internet access","Weather forecasting"}, 2),
                    new Question("How many satellites does Starlink plan to deploy?", new[]{"Few hundred","One thousand","Over 40,000","Less than 500"}, 2),
                    new Question("Starlink satellites use which technology to adjust orbit?", new[]{"Liquid fuel","Solar sails","Ion thrusters","Chemical boosters"}, 2),
                    new Question("Which rocket is used for Starlink launches?", new[]{"Atlas V","Delta IV","Falcon 9","Ariane 5"}, 2),
                };
            default:
                return new List<Question>();
        }
    }
}
