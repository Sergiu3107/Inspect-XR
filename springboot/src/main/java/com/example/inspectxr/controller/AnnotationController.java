package com.example.inspectxr.controller;

import com.example.inspectxr.dtos.*;
import com.example.inspectxr.service.AnnotationService;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@CrossOrigin(origins = "http://localhost:8081")
@RestController
@RequestMapping("/annotation")
public class AnnotationController {

    private final AnnotationService annotationService;

    @Autowired
    public AnnotationController(AnnotationService annotationService) {
        this.annotationService = annotationService;
    }

    @GetMapping("/point/{id}")
    public ResponseEntity<?> getPointAnnotationById(@PathVariable Long id) {
        try {
            PointAnnotationResponseDTO annotation = annotationService.getPointAnnotationById(id);
            return ResponseEntity.ok(annotation);
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Error: " + e.getMessage());
        }
    }

    @GetMapping("/paint/{id}")
    public ResponseEntity<?> getPaintAnnotationById(@PathVariable Long id) {
        try {
            PaintAnnotationResponseDTO annotation = annotationService.getPaintAnnotationById(id);
            return ResponseEntity.ok(annotation);
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Error: " + e.getMessage());
        }
    }

    @PostMapping("/point")
    public ResponseEntity<?> addPointAnnotation(@Valid @RequestBody PointAnnotationRequestDTO dto) {
        try {
            AnnotationResponseDTO saved = annotationService.addPointAnnotation(dto);
            return ResponseEntity.status(201).body(saved);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body("Error: " + e.getMessage());
        }
    }

    @PutMapping("/point/{id}")
    public ResponseEntity<?> updatePointAnnotation(@PathVariable Long id, @Valid @RequestBody PointAnnotationRequestDTO dto) {
        try {
            AnnotationResponseDTO updated = annotationService.updatePointAnnotation(id, dto);
            return ResponseEntity.ok(updated);
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Error: " + e.getMessage());
        }
    }


    @PostMapping("/paint")
    public ResponseEntity<?> addPaintAnnotation(@Valid @RequestBody PaintAnnotationRequestDTO dto) {
        try {
            AnnotationResponseDTO saved = annotationService.addPaintAnnotation(dto);
            return ResponseEntity.status(201).body(saved);
        } catch (Exception e) {
            return ResponseEntity.badRequest().body("Error: " + e.getMessage());
        }
    }

    @PutMapping("/paint/{id}")
    public ResponseEntity<?> updatePaintAnnotation(@PathVariable Long id, @Valid @RequestBody PaintAnnotationRequestDTO dto) {
        try {
            AnnotationResponseDTO updated = annotationService.updatePaintAnnotation(id, dto);
            return ResponseEntity.ok(updated);
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Error: " + e.getMessage());
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<?> deleteAnnotation(@PathVariable Long id) {
        try {
            annotationService.deleteAnnotation(id);
            return ResponseEntity.noContent().build();
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Error: " + e.getMessage());
        }
    }

    @GetMapping("/point/session/{sessionId}")
    public ResponseEntity<List<PointAnnotationResponseDTO>> getPointAnnotationsBySession(@PathVariable Long sessionId) {
        List<PointAnnotationResponseDTO> annotations = annotationService.getPointAnnotationsBySession(sessionId);
        return ResponseEntity.ok(annotations);
    }

    @GetMapping("/paint/session/{sessionId}")
    public ResponseEntity<List<PaintAnnotationResponseDTO>> getPaintAnnotationsBySession(@PathVariable Long sessionId) {
        List<PaintAnnotationResponseDTO> annotations = annotationService.getPaintAnnotationsBySession(sessionId);
        return ResponseEntity.ok(annotations);
    }
}
