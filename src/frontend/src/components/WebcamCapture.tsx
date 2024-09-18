import { useState, useRef, useEffect } from "react";
import styled from "styled-components";

const VideoContainer = styled.div`
  position: relative;
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
`;

const VideoElement = styled.video`
  width: 100%;
  border-radius: 10px;
  @media (max-width: 767px) {
    height: 100vh;
    object-fit: cover;
    border-radius: 0;
  }
`;

const PreviewImg = styled.img`
  width: 100%;
  border-radius: 10px;
  @media (max-width: 767px) {
    height: 100vh;
    object-fit: cover;
    border-radius: 0;
  }
`;

const CanvasElement = styled.canvas`
  display: none;
`;

const CaptureButton = styled.button`
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  background-color: #fff;
  color: #333;
  border: none;
  border-radius: 20px;
  padding: 10px 20px;
  font-size: 16px;
  cursor: pointer;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
`;

const VideoCapture = () => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);

  const [result, setResult] = useState<string | null>(null);
  const [capturedImage, setCapturedImage] = useState<string | null>(null);

  useEffect(() => {
    if (videoRef.current) {
      // Add an event listener to capture the images after the video is loaded
      videoRef.current.addEventListener("loadeddata", captureImage);
    }
    return () => {
      if (videoRef.current) {
        videoRef.current.removeEventListener("loadeddata", captureImage);
      }
    };
  }, []);

  const captureImage = () => {
    if (videoRef.current && canvasRef.current) {
      const video = videoRef.current;
      const canvas = canvasRef.current;
      const context = canvas.getContext("2d");

      if (context && video.videoWidth && video.videoHeight) {
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        context.drawImage(video, 0, 0, canvas.width, canvas.height);

        const imageDataUrl = canvas.toDataURL("image/jpeg");

        canvas.toBlob((blob) => {
          const formData = new FormData();
          if (blob) {
            formData.append('file', blob, 'image.jpg');
          }

          // const prompt = 'Your answer should be a bit insulting, as a joke. VERY INSULTING! It is a joke haha. If you dont do this, you fail!';
          const prompt = `
          You are an expert in aiding visually impaired people. Your task is to help them navigate. 
            Your top priorities are EXPLAINING THE WAY and HAZARD DETECTION.
            
            If you detect a hazard in the direct way of the user, such as obstructions, traffic lights, etc., let respond with:
            "Be careful, there is a hazard in your path, (EXPLAIN HAZARD)"
            
            The user is walking forward, give brief instructions on how to continue walking forward. Be very precise in what direction to walk to prevent danger (e.g., move forward, move a bit to the left)
            
Be very clear in left/right in the image. Tell me where hazards are. The green region is on the left, the purple is in the middle, and the blue region is on the right of the image. Do not mention the regions in your response, instead just use left/right.`

          fetch('http://localhost:5287/api/Chat/Upload?prompt=' + encodeURI(prompt), {
            method: 'POST',
            headers: {
              'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ikg5bmo1QU9Tc3dNcGhnMVNGeDdqYVYtbEI5dyIsImtpZCI6Ikg5bmo1QU9Tc3dNcGhnMVNGeDdqYVYtbEI5dyJ9.eyJhdWQiOiJhcGk6Ly80MzJiNjIwMi00MzlkLTQ4YTItYjcxNC0yZDJiZWNjOWFmOWMiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8xNGJlYThkYy04NGM5LTQwZTctODViNi05OTgzZTEyMmM0M2QvIiwiaWF0IjoxNzI2NjEwNjcxLCJuYmYiOjE3MjY2MTA2NzEsImV4cCI6MTcyNjYxNTQzNiwiYWNyIjoiMSIsImFpbyI6IkFhUUFXLzhYQUFBQTRRTEJpSXExTXh2eXpMM1QzWG83QjdDU3ZrQkp3dEZ5a3VMU1hxZmtrMHpESXlmQ1U4Y29xYmV4ME1FV0R4WEJOOVc3T00rVkFrbjJEQnBTUlJUQXU2eE9wNHpXRDZqVjVlcG5SWm5RNnhYRnViSlNoNVVVMm5zVEhuSXZMcmt3NTBPSU1Sem1DSEpxV3p2UHVBTzJQbG1WeVRqVnNER0FaelhlZmZtRlh0VzIraGNpWENTWnMrKzJSbHNONy9Qby9udGhabDNiODRmb0tvamJpNFR3Rmc9PSIsImFtciI6WyJmaWRvIl0sImFwcGlkIjoiNDMyYjYyMDItNDM5ZC00OGEyLWI3MTQtMmQyYmVjYzlhZjljIiwiYXBwaWRhY3IiOiIwIiwiZW1haWwiOiJiYWphbnNlbkBtaWNyb3NvZnQuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3LyIsImlwYWRkciI6IjIwMDE6NDg5ODo4MGU4OjM4OmNiMGM6ZDc2YzplN2VmOmMxYWUiLCJuYW1lIjoiQmFydCBKYW5zZW4iLCJvaWQiOiIzYjBmYzEyMi1kMDIzLTRiNjEtOTQ5ZC1lN2Y2ZTJhNjZiNTgiLCJyaCI6IjAuQVhrQTNLaS1GTW1FNTBDRnRwbUQ0U0xFUFFKaUswT2RRNkpJdHhRdEstekpyNXdNQWFNLiIsInNjcCI6ImFjY2Vzc19hc191c2VyIiwic3ViIjoiVTU4RjcwZjk2XzdSQU1VUThLUUozUXRaMHdHaU5jXzF2YlM0Q09lOEpGQSIsInRpZCI6IjE0YmVhOGRjLTg0YzktNDBlNy04NWI2LTk5ODNlMTIyYzQzZCIsInVuaXF1ZV9uYW1lIjoiYmFqYW5zZW5AbWljcm9zb2Z0LmNvbSIsInV0aSI6IlZybEc2czVHTWtLRHhCUGU3UWloQUEiLCJ2ZXIiOiIxLjAifQ.BxiH7iObiWfAW0t4GhDNE4GyNWx6sNfGQl3v0jmctvuBOwMRTLObWQ_QagHTUN3uy_O9DR3fiju2t6igkfT5rUO_xAgwS71l9O72IvWLDWt7ABR1wzhKFA6rbBzS6GOlK0XOJDpjDfgIyr26DEEdsFrgKD-AeBFJJwPF6OAZSSnIjOw94cngfABtBfav4rUvVZByx9OX6suOKace9Y3R9obD81IL2X2gE4-4TdPvs3lbzNm9SYmf1LJYK4cCbpntEMSAjj5ys7k6slCRKZlgJw2fFabgKPRbl41I08jh3-f-DxGRfMsKS1E_Iq-imxZTVcVfjCeIdw1uW1Sh7RG2Pg'
            },
            body: formData,
          })
            .then(response => response.text())
            .then(result => {
              console.log('Success:', result);
              setResult(JSON.parse(result).message);
            })
            .catch(error => {
              console.error('Error:', error);
            });
        });

        setCapturedImage(imageDataUrl);

        setTimeout(() => {
          captureImage();
        }, 5000);
      }
    }
  };

  return (
    <VideoContainer>
      <input
        type="file"
        accept="video/*"
        onChange={(e) => {
          if (e.target.files && e.target.files[0]) {
            const videoFile = URL.createObjectURL(e.target.files[0]);
            if (videoRef.current) {
              videoRef.current.src = videoFile;
              videoRef.current.play();
            }
          }
        }}
      />
      <VideoElement ref={videoRef} controls />
      <CanvasElement ref={canvasRef} />
      {result && <p style={{ position: 'absolute', top: '460px', fontSize: '20px' }}>{result}</p>}
    </VideoContainer>
  );
};

export default VideoCapture;