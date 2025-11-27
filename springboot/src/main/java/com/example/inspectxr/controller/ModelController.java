package com.example.inspectxr.controller;

import com.example.inspectxr.dtos.ModelRequestDTO;
import com.example.inspectxr.dtos.ModelResponseDTO;
import com.example.inspectxr.service.ModelService;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@CrossOrigin(origins = "http://localhost:8081")
@RestController
@RequestMapping("/model")
public class ModelController {


    private final ModelService modelService;

    @Autowired
    public ModelController(ModelService modelService) {
        this.modelService = modelService;
    }

    @GetMapping("/{id}")
    public ResponseEntity<?> getModelById(@PathVariable Long id) {
        ModelResponseDTO dto = modelService.getModelById(id);
        return new ResponseEntity<>(dto, HttpStatus.OK);
    }


    @PostMapping(consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    public ResponseEntity<?> addModel(
            @RequestPart("metadata") @Valid ModelRequestDTO dto,
            @RequestPart("file") MultipartFile file) {
        try {
            ModelResponseDTO model = modelService.addModel(dto, file);
            return new ResponseEntity<>(model, HttpStatus.CREATED);
        } catch (Exception e) {
            e.printStackTrace();
            return new ResponseEntity<>("Error: " + e.getMessage(), HttpStatus.BAD_REQUEST);
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<?> deleteModel(@PathVariable Long id) {
        try {
            modelService.deleteModel(id);
            return new ResponseEntity<>(HttpStatus.NO_CONTENT);
        } catch (Exception e) {
            return new ResponseEntity<>("Error: " + e.getMessage(), HttpStatus.NOT_FOUND);
        }
    }

    @GetMapping("/owner/{ownerId}")
    public ResponseEntity<?> getModelsByOwnerId(@PathVariable Long ownerId) {
        try {
            return new ResponseEntity<>(modelService.getModelsByOwnerId(ownerId), HttpStatus.OK);
        } catch (Exception e) {
            return new ResponseEntity<>("Error: " + e.getMessage(), HttpStatus.NOT_FOUND);
        }
    }

}
